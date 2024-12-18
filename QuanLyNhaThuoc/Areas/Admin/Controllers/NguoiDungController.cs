using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.Models;
using Microsoft.Data.SqlClient; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
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
            var usersQuery = db.NguoiDungs.Include(u => u.MaVaiTroNavigation).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery.Where(u => u.TenNguoiDung.Contains(searchString) || u.Email.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(roleFilter) && int.TryParse(roleFilter, out int roleId))
            {
                usersQuery = usersQuery.Where(u => u.MaVaiTro == roleId);
            }
            if (!string.IsNullOrEmpty(statusFilter))
            {
                usersQuery = usersQuery.Where(u => u.TrangThai == statusFilter);
            }

            int totalItems = usersQuery.Count();

            var users = usersQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(users);
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
            new SqlParameter("@Email", Email),
            new SqlParameter("@SoDienThoai", SoDienThoai),
        };

                db.Database.ExecuteSqlRaw("EXEC sp_ThemNguoiDungNhanVien @TenNguoiDung, @Email, @SoDienThoai", parameters);

                return RedirectToAction("Index");
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627) // Mã lỗi vi phạm ràng buộc UNIQUE
                {
                    ModelState.AddModelError("", "Email hoặc số điện thoại đã tồn tại.");
                }
                else
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi thêm người dùng. Vui lòng thử lại.");
                }

                _logger.LogError(sqlEx, "Lỗi hệ thống khi thêm người dùng.");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo người dùng.");
                ModelState.AddModelError("", "Đã xảy ra lỗi không xác định. Vui lòng thử lại.");
                return View();
            }
        }
        [Route("Edit/{id}")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Lấy thông tin người dùng
            var user = db.NguoiDungs.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            // Lấy danh sách vai trò
            ViewBag.RoleList = db.VaiTros.ToList(); 

            return View(user);
        }


        [Route("Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string TenNguoiDung, string Email, string SoDienThoai, string TrangThai, int? MaVaiTro)
        {
            try
            {
                // Thiết lập các tham số cho stored procedure
                var parameters = new[]
                {
            new SqlParameter("@MaNguoiDung", id),
            new SqlParameter("@TenNguoiDung", (object)TenNguoiDung ?? DBNull.Value),
            new SqlParameter("@Email", (object)Email ?? DBNull.Value),
            new SqlParameter("@SoDienThoai", (object)SoDienThoai ?? DBNull.Value),
            new SqlParameter("@TrangThai", (object)TrangThai ?? DBNull.Value),
            new SqlParameter("@MaVaiTro", MaVaiTro ?? (object)DBNull.Value)
        };

                // Gọi stored procedure
                db.Database.ExecuteSqlRaw("EXEC sp_CapNhatNguoiDung @MaNguoiDung, @TenNguoiDung, @Email, @SoDienThoai, @TrangThai, @MaVaiTro", parameters);

                return RedirectToAction("Index");
            }
            catch (SqlException sqlEx)
            {
                ModelState.AddModelError("", "Không thể cập nhật thông tin người dùng. Vui lòng kiểm tra lại.");

                _logger.LogError(sqlEx, "Lỗi hệ thống khi cập nhật người dùng.");
                return View();
            }

        }


        [Route("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return Json(new { success = false, message = "Không có người dùng nào được chọn để xóa." });
                }

                foreach (var id in ids)
                {
                
                    using (var connection = new SqlConnection(db.Database.GetConnectionString()))
                    {
                        using (var command = new SqlCommand("sp_XoaNguoiDungNhanVien", connection))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;

                            command.Parameters.Add(new SqlParameter("@MaNguoiDung", id));

                            var returnValue = new SqlParameter();
                            returnValue.Direction = System.Data.ParameterDirection.ReturnValue;
                            command.Parameters.Add(returnValue);

                            connection.Open();
                            command.ExecuteNonQuery();

                            var result = (int)returnValue.Value;

                            if (result == 0)
                            {
                                return Json(new { success = false, message = $"Không thể xóa người dùng có mã {id}. Kiểm tra lại vai trò và trạng thái." });
                            }
                        }
                    }
                }

                return Json(new { success = true, message = "Người dùng đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }
        [Route("ResetPassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ResetPassword(int[] ids)
        {
            try
            {
                if (ids.Length == 0)
                {
                    return Json(new { success = false, message = "Không có người dùng nào được chọn để reset mật khẩu." });
                }

                foreach (var id in ids)
                {
                    // Sử dụng ExecuteSqlRawAsync để chạy bất đồng bộ
                    await db.Database.ExecuteSqlRawAsync("EXEC sp_ResetMatKhauNguoiDung @MaNguoiDung = {0}", id);
                }

                return Json(new { success = true, message = "Mật khẩu đã được reset thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi reset mật khẩu: " + ex.Message });
            }
        }



    }
}
