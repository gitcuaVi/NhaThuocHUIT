using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.Models;
using Microsoft.Data.SqlClient; 
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class NguoiDungController : Controller
    {
        private readonly QL_NhaThuocContext db;
        private readonly ILogger<NguoiDungController> _logger;

        public NguoiDungController(QL_NhaThuocContext context, ILogger<NguoiDungController> logger)
        {
            db = context;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index(string searchString, string roleFilter, string statusFilter)
        {
            var users = from u in db.NguoiDungs select u;

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.TenNguoiDung.Contains(searchString) || u.Email.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(roleFilter))
            {
                if (int.TryParse(roleFilter, out int roleId))
                {
                    users = users.Where(u => u.MaVaiTro == roleId);
                }
            }


            if (!String.IsNullOrEmpty(statusFilter))
            {
                users = users.Where(u => u.TrangThai == statusFilter);
            }

            return View(users.ToList());
        }
        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string TenNguoiDung, string Password, string Email, string SoDienThoai, int MaVaiTro, string TrangThai)
        {
            try
            {
                // Thiết lập các tham số cho stored procedure
                var parameters = new[]
                {
                    new SqlParameter("@TenNguoiDung", TenNguoiDung),
                    new SqlParameter("@Password", Password),
                    new SqlParameter("@Email", Email),
                    new SqlParameter("@SoDienThoai", SoDienThoai),
                };

               
                db.Database.ExecuteSqlRaw("EXEC sp_ThemNguoiDungNhanVien @TenNguoiDung, @Password, @Email, @SoDienThoai", parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo người dùng.");
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi thêm người dùng.";
                return View();
            }
        }
    }
}
