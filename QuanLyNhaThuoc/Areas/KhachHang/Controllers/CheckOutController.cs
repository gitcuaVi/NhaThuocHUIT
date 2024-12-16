using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Services.VnPay;
using Microsoft.Data.SqlClient;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.Areas.KhachHang.Services.Momo;

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

        [HttpGet("PaymentCallBackMomo")]
        public async Task<IActionResult> PaymentCallBackMomo([FromQuery] PaymentModel model)
        {
            var maGiaoDich = Request.Query["vnp_TxnRef"].ToString();

            var requestId = Request.Query["requestId"].ToString();
            var response = _momoService.PaymentExecuteMomo(Request.Query);
            if (Request.Query["resultCode"] == "0")
            {

                var paramMaGiaoDich = new SqlParameter("@MaGiaoDich", maGiaoDich);
                var paramRequestId = new SqlParameter("@RequestId", requestId);  

                var result = await db.Database.ExecuteSqlRawAsync("EXEC sp_CapNhatTrangThaiThanhToan @MaGiaoDich", paramRequestId);

                TempData["Success"] = "Thanh toán thành công!";
                return RedirectToAction("ThongTinDonHang", new { maDonHang = Convert.ToInt32(requestId) });
            }
            else
            {

                TempData["Error"] = "Đã hủy giao dịch Momo";
                return RedirectToAction("Cart", "SanPham");
            }
        }

        [HttpGet("PaymentCallbackVnpay")]
        public async Task<IActionResult> PaymentCallbackVnpay([FromQuery] PaymentModel model)
        {
            var maGiaoDich = Request.Query["vnp_TxnRef"].ToString();
            // Kiểm tra nếu mã giao dịch  không hợp lệ
            if (maGiaoDich == null)
            {
                TempData["Error"] = "Mã đơn hàng không hợp lệ.";
                return RedirectToAction("Cart", "SanPham");
            }

            // Thực thi việc kiểm tra giao dịch qua VNPAY
            var response = _vnPayService.PaymentExecute(Request.Query);

            // Kiểm tra trạng thái giao dịch từ VNPAY
            if (response == null || !response.Success || response.VnPayResponseCode != "00")
            {
                TempData["Error"] = "Giao dịch thất bại.";
                return RedirectToAction("Cart", "SanPham");
            }

            // Cập nhật trạng thái thanh toán cho đơn hàng trong cơ sở dữ liệu
            var paramMaGiaoDich = new SqlParameter("@MaGiaoDich", maGiaoDich);

            var result = await db.Database
                                  .ExecuteSqlRawAsync("EXEC sp_CapNhatTrangThaiThanhToan @MaGiaoDich", paramMaGiaoDich);
            TempData["Success"] = "Thanh toán thành công!";
            return RedirectToAction("OrderDetails", "SanPham", new { maDonHang = maGiaoDich });

        }
        [HttpGet("ThongTinDonHang")]
        public async Task<IActionResult> ThongTinDonHang(int maDonHang)
        {
            try
            {
                // Kiểm tra mã giao dịch có hợp lệ hay không
                if (maDonHang <= 0)
                {
                    TempData["Error"] = "Mã đơn hàng không hợp lệ.";
                    return RedirectToAction("Index", "LichSuDonHang");
                }

                // Gọi stored procedure và ánh xạ dữ liệu vào model
                var paramMaDonHang = new SqlParameter("@MaDonHang", maDonHang);

                // Sử dụng FromSqlRaw để lấy dữ liệu
                var donHang = await db.ThongTinDatHangViewModels
                    .FromSqlRaw("EXEC sp_GetThongTinDatHang @MaDonHang", paramMaDonHang)
                    .FirstOrDefaultAsync();

                // Nếu không tìm thấy đơn hàng
                if (donHang == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin đơn hàng.";
                    return RedirectToAction("Index", "LichSuDonHang");
                }

                // Trả về view với model
                return View(donHang);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Đã xảy ra lỗi khi lấy thông tin đơn hàng: " + ex.Message;
                return RedirectToAction("Index", "LichSuDonHang");
            }
        }

    }


}
