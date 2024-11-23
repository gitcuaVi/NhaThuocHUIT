using QuanLyNhaThuoc.Areas.KhachHang.Models.VnPay;
using QuanLyNhaThuoc.Libarys;
using QuanLyNhaThuoc.Areas.KhachHang.Services;
using QuanLyNhaThuoc.Areas.KhachHang.Services.VnPay;

namespace QuanLyNhaThuoc.KhachHang.Services.VnPay
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);

            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["Vnpay:PaymentBackReturnUrl"];

            // Thêm các tham số vào VNPAY
            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);

            // Thay vì sử dụng tick, truyền MaDonHang vào vnp_TxnRef
            pay.AddRequestData("vnp_TxnRef", model.OrderId);  // Thay "tick" bằng OrderId

            // Tạo URL thanh toán
            var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
            return paymentUrl;
        }


        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            try
            {
                var pay = new VnPayLibrary();

                // Get full response data as a PaymentResponseModel
                var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

                // Analyze transaction status directly from the response model
                response.Success = response.VnPayResponseCode == "00";

                return response;
            }
            catch (Exception ex)
            {
                return new PaymentResponseModel
                {
                    Success = false,
                    VnPayResponseCode = "Error",
                    OrderDescription = "Lỗi khi xử lý thanh toán: " + ex.Message
                };
            }
        }


    }

}
