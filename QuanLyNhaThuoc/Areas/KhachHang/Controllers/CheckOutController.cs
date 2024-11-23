using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Services.VnPay;
using Microsoft.Data.SqlClient;

namespace QuanLyNhaThuoc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang/[controller]")]
    public class CheckOutController : Controller
    {
        private readonly QL_NhaThuocContext db;
        private readonly IMomoService _momoService;
        private readonly IOptions<MomoOptionModel> _options;  // Inject IOptions
        private readonly IVnPayService _vnPayService;

        public CheckOutController(QL_NhaThuocContext context, IMomoService momoService, IOptions<MomoOptionModel> options, IVnPayService vnPayService)
        {
            db = context;
            _momoService = momoService;
            _options = options;  // Initialize _options
            _momoService = momoService;
            _vnPayService = vnPayService;
        }

        [HttpGet("PaymentCallBack")]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _momoService.PaymentExecuteMomo(HttpContext.Request.Query);
         var requestQuery = HttpContext.Request.Query;
            if (requestQuery["resultCode"]==0)
            {

            }   
            else
            {
                TempData["success"] = "Đã hủy giao dịch Momo";
                return RedirectToAction("Cart","SanPham");
            }

            return View(response);


        }
        [HttpGet("PaymentCallbackVnpay")]
        public async Task<IActionResult> PaymentCallbackVnpay() // Đổi từ IActionResult thành Task<IActionResult>
        {
            try
            {
                // Thực hiện xử lý kết quả thanh toán qua VNPAY
                var response = _vnPayService.PaymentExecute(Request.Query);
                if (Request.Query == null || !Request.Query.Any())
                {
                    TempData["Error"] = "Dữ liệu phản hồi từ VNPAY không hợp lệ.";
                    return RedirectToAction("Cart", "SanPham");
                }
                if (response == null)
                {
                    TempData["Error"] = "Không thể xử lý phản hồi từ VNPAY.";
                    return RedirectToAction("Cart", "SanPham");
                }

                // Kiểm tra trạng thái giao dịch dựa trên mã phản hồi từ VNPAY
                if (response.Success && response.VnPayResponseCode == "00") // 00: Giao dịch thành công
                {
                    TempData["Success"] = "Giao dịch VNPAY thành công!";
                    int maDonHang = int.Parse(response.OrderId);

                    // Gọi procedure cập nhật trạng thái thanh toán
                    var paramMaDonHang = new SqlParameter("@MaDonHang", maDonHang);
                    await db.Database.ExecuteSqlRawAsync("EXEC sp_CapNhatTrangThaiThanhToan @MaDonHang", paramMaDonHang);

                    return RedirectToAction("Index", "LichSuDonHang");
                }
                else
                {
                    TempData["Error"] = $"Giao dịch thất bại. Mã lỗi: {response.VnPayResponseCode}";
                    return RedirectToAction("Cart", "SanPham");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Đã xảy ra lỗi khi xử lý kết quả thanh toán: " + ex.Message;
                return RedirectToAction("Cart", "SanPham");
            }
        }


    }
}
