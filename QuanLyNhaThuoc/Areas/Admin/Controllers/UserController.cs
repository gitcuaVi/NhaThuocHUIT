using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;
using System.Linq;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class UserController : Controller
    {
        private readonly QL_NhaThuocContext db;
        private readonly ILogger<UserController> _logger;

        public UserController(QL_NhaThuocContext context, ILogger<UserController> logger)
        {
            db = context;
            _logger = logger;
        }
      
        [HttpGet("GetPendingOrders")]
        public IActionResult GetPendingOrders()
        {
            try
            {
                // Lấy thông báo đơn hàng
                var pendingOrders = db.DonHangs
                    .Where(d => d.TrangThai == "Chờ xác nhận")
                    .OrderByDescending(d => d.NgayDatHang)
                    .Select(d => new
                    {
                        Type = "Order",
                        d.MaDonHang,
                        d.NgayDatHang,
                        KhachHang = d.KhachHang != null ? d.KhachHang.TenKhachHang : "Không rõ"
                    })
                    .ToList();

                // Lấy thông báo tồn kho
                var lowStockItems = db.TonKhos
                    .Where(tk => tk.SoLuongTon < tk.SoLuongCanhBao)
                    .OrderBy(tk => tk.NgayGioCapNhat)
                    .Select(tk => new
                    {
                        Type = "Stock",
                        tk.MaThuoc,
                        TenThuoc = tk.MaThuocNavigation != null ? tk.MaThuocNavigation.TenThuoc : "Không rõ",
                        tk.SoLuongTon,
                        tk.SoLuongCanhBao
                    })
                    .ToList();

                // Kết hợp dữ liệu
                var notifications = pendingOrders.Concat<object>(lowStockItems);

                return PartialView("_PendingOrdersNotification", notifications);
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi khi tải thông báo: {ex.Message}");
            }
        }

        [HttpGet("Profile")]
        public IActionResult Profile()
        {
            var maNguoiDungClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (maNguoiDungClaim != null)
            {
                int maNguoiDung = int.Parse(maNguoiDungClaim.Value);
                var userInfo = db.ThongTinCaNhanView.FirstOrDefault(u => u.MaNguoiDung == maNguoiDung);

                if (userInfo != null)
                {
                    ViewBag.UserName = userInfo.TenNguoiDung;

                    return View(userInfo);
                }
                else
                {
                    return NotFound("Không tìm thấy thông tin người dùng.");
                }
            }
            else
            {
                return Unauthorized("Bạn chưa đăng nhập.");
            }
        }
        [HttpGet("DisplayName")]
        public IActionResult DisplayName()
        {
            var maNguoiDungClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (maNguoiDungClaim != null)
            {
                int maNguoiDung = int.Parse(maNguoiDungClaim.Value);
                var userInfo = db.ThongTinCaNhanView.FirstOrDefault(u => u.MaNguoiDung == maNguoiDung);

                if (userInfo != null)
                {
                    ViewBag.UserName = userInfo.TenNguoiDung;

                    return View(userInfo);
                }
                else
                {
                    return NotFound("Không tìm thấy thông tin người dùng.");
                }
            }
            else
            {
                return Unauthorized("Bạn chưa đăng nhập.");
            }
        }

        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmNewPassword)
        {
            var maNguoiDungClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (maNguoiDungClaim == null)
            {
                return Unauthorized("Bạn chưa đăng nhập.");
            }

            int maNguoiDung = int.Parse(maNguoiDungClaim.Value);

            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@MaNguoiDung", maNguoiDung),
                    new SqlParameter("@OldPassword", oldPassword),
                    new SqlParameter("@NewPassword", newPassword),
                    new SqlParameter("@ConfirmNewPassword", confirmNewPassword)
                };

                db.Database.ExecuteSqlRaw("EXEC sp_DoiMatKhau @MaNguoiDung, @OldPassword, @NewPassword, @ConfirmNewPassword", parameters);

                TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công.";
                return RedirectToAction("ChangePassword");
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi thay đổi mật khẩu: " + ex.Message;
            }
            return RedirectToAction("ChangePassword");
        }
    }
}
