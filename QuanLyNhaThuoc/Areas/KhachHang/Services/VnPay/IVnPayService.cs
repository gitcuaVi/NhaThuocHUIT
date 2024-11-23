using QuanLyNhaThuoc.Areas.KhachHang.Models.VnPay;

namespace QuanLyNhaThuoc.Areas.KhachHang.Services.VnPay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
