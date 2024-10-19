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
        public IActionResult Index(string searchString, string roleFilter, string statusFilter, int page = 1, int pageSize = 6)
        {
            ViewBag.RoleList = db.VaiTros.ToList();
            var users = from u in db.NguoiDungs select u;

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.TenNguoiDung.Contains(searchString) || u.Email.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(roleFilter) && int.TryParse(roleFilter, out int roleId))
            {
                users = users.Where(u => u.MaVaiTro == roleId);
            }

            // Filter by status (Active/Inactive)
            if (!String.IsNullOrEmpty(statusFilter))
            {
                users = users.Where(u => u.TrangThai == statusFilter);
            }
            int totalItems = users.Count();
            users = users.Skip((page - 1) * pageSize).Take(pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

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
        [Route("Edit/{id}")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
        //lấy tt
            var user = db.NguoiDungs.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Route("Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string TenNguoiDung, string Email, string SoDienThoai, string TrangThai)
        {
            try
            {
                // Thiết lập các tham số stored procedure
                var parameters = new[]
                {
            new SqlParameter("@MaNguoiDung", id),
            new SqlParameter("@TenNguoiDung", (object)TenNguoiDung ?? DBNull.Value),
            new SqlParameter("@Email", (object)Email ?? DBNull.Value),
            new SqlParameter("@SoDienThoai", (object)SoDienThoai ?? DBNull.Value),
            new SqlParameter("@TrangThai", (object)TrangThai ?? DBNull.Value)
        };

                // Gọi stored procedure
                db.Database.ExecuteSqlRaw("EXEC sp_CapNhatThongTinNhanVien @MaNguoiDung, @TenNguoiDung, @Email, @SoDienThoai, @TrangThai", parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chỉnh sửa thông tin người dùng.");
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi chỉnh sửa thông tin người dùng.";
                return View();
            }
        }

    }
}
