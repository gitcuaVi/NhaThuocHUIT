
using Microsoft.AspNetCore.Mvc; 
using System.Security.Claims;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyNhaThuoc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace QuanLyNhaThuoc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang/[controller]")]
    public class UserDHController : Controller
    {
        private readonly QL_NhaThuocContext _context;

        public UserDHController(QL_NhaThuocContext context)
        {
            _context = context;
        }

        // Trang hiển thị thông tin người dùng
        [HttpGet("Profile")]
        public IActionResult Profile()
        {
            var maNguoiDungClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (maNguoiDungClaim != null)
            {
                int maNguoiDung = int.Parse(maNguoiDungClaim.Value);

                // Khởi tạo đối tượng SqlParameter và thiết lập giá trị
                SqlParameter param = new SqlParameter("@MaNguoiDung", SqlDbType.Int);
                param.Value = maNguoiDung;

                // Gọi Stored Procedure
                var userInfo = _context.ThongTinKhachHangViewModel
                    .FromSqlRaw("EXEC sp_HienThiThongTinKhachHang @MaNguoiDung", param)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (userInfo != null)
                {
                    return View(userInfo);  // Hiển thị thông tin khách hàng đã đăng nhập
                }
                else
                {
                    return NotFound("Không tìm thấy thông tin khách hàng.");
                }
            }
            else
            {
                return RedirectToAction("Login");  // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }
        }
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(
    string tenKhachHang,
    string gioiTinh,
    string diaChi,
    string soDienThoai,
    DateTime ngaySinh,
    string email,
    string password,
    string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu không khớp.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tạo các tham số cho stored procedure
                    var parameters = new[]
                    {
                new SqlParameter("@TenKhachHang", tenKhachHang),
                new SqlParameter("@GioiTinh", gioiTinh),
                new SqlParameter("@DiaChi", diaChi),
                new SqlParameter("@SoDienThoai", soDienThoai),
                new SqlParameter("@NgaySinh", ngaySinh),
                new SqlParameter("@Email", email),
                new SqlParameter("@Password", password)
            };

                    // Gọi stored procedure
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC sp_DangKyTaiKhoan @TenKhachHang, @GioiTinh, @DiaChi, @SoDienThoai, @NgaySinh, @Email, @Password",
                        parameters);

                    // Đăng ký thành công, chuyển hướng đến trang đăng nhập
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đăng ký tài khoản.");
                }
            }

            // Nếu có lỗi, hiển thị lại form đăng ký
            return View();
        }


        // Trang đăng nhập (GET)
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var paramUsername = new SqlParameter("@Username", username);
                    var paramPassword = new SqlParameter("@Password", password);

                    using (var connection = (SqlConnection)_context.Database.GetDbConnection())
                    {
                        await connection.OpenAsync();
                        using (var command = new SqlCommand("sp_LoginKhachHang", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(paramUsername);
                            command.Parameters.Add(paramPassword);

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    int maNguoiDung = reader.GetInt32(0);
                                    int? maVaiTro = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1);

                                    if (maNguoiDung > 0 && maVaiTro == 3)
                                    {
                                        int maKhachHang = GetMaKhachHang(maNguoiDung);
                                        if (maKhachHang == -1)
                                        {
                                            ViewBag.ErrorMessage = "Không tìm thấy mã Khách Hàng tương ứng.";
                                            return View();
                                        }

                                        var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name, username),
                                    new Claim("UserId", maNguoiDung.ToString()),
                                    new Claim("Role", maVaiTro.ToString()),
                                    new Claim("MaKhachHang", maKhachHang.ToString())
                                };

                                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                                        return RedirectToAction("Profile");
                                    }
                                }
                            }
                        }
                    }

                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng hoặc tài khoản không hoạt động.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình đăng nhập.");
                }
            }
            return View();
        }


        [HttpGet("EditProfile")]
        public IActionResult EditProfile()
        {
            var maNguoiDungClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (maNguoiDungClaim != null)
            {
                int maNguoiDung = int.Parse(maNguoiDungClaim.Value);

                SqlParameter param = new SqlParameter("@MaNguoiDung", SqlDbType.Int) { Value = maNguoiDung };

                var userInfo = _context.ThongTinKhachHangViewModel
                    .FromSqlRaw("EXEC sp_HienThiThongTinKhachHang @MaNguoiDung", param)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (userInfo != null)
                {
                    return View(userInfo);
                }
                else
                {
                    return NotFound("Không tìm thấy thông tin khách hàng.");
                }
            }
            return RedirectToAction("Login");
        }

        [HttpPost("EditProfile")]
        public async Task<IActionResult> EditProfile(ThongTinKhachHangViewModel model)
        {
            var maNguoiDungClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (maNguoiDungClaim != null)
            {
                int maNguoiDung = int.Parse(maNguoiDungClaim.Value);

                var parameters = new[]
                {
            new SqlParameter("@MaNguoiDung", maNguoiDung),
            new SqlParameter("@Email", model.Email ?? (object)DBNull.Value),
            new SqlParameter("@SoDienThoai", model.SoDienThoai ?? (object)DBNull.Value),
            new SqlParameter("@TenKhachHang", model.TenKhachHang ?? (object)DBNull.Value),
            new SqlParameter("@GioiTinh", model.GioiTinh ?? (object)DBNull.Value),
            new SqlParameter("@DiaChi", model.DiaChi ?? (object)DBNull.Value),
            new SqlParameter("@NgaySinh", model.NgaySinh ?? (object)DBNull.Value)
        };

                await _context.Database.ExecuteSqlRawAsync("EXEC sp_CapNhatThongTinKhachHang @MaNguoiDung, @Email, @SoDienThoai, @TenKhachHang, @GioiTinh, @DiaChi, @NgaySinh", parameters);

                return RedirectToAction("Profile");
            }

            return RedirectToAction("Login");
        }
        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu mới và mật khẩu xác nhận không khớp.");
                return View();
            }

            var maNguoiDungClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (maNguoiDungClaim != null)
            {
                int maNguoiDung = int.Parse(maNguoiDungClaim.Value);

                try
                {
                    // Kiểm tra mật khẩu hiện tại của người dùng
                    var paramUsername = new SqlParameter("@Username", User.Identity.Name);
                    var paramCurrentPassword = new SqlParameter("@Password", currentPassword);

                    using (var connection = (SqlConnection)_context.Database.GetDbConnection())
                    {
                        await connection.OpenAsync();
                        using (var command = new SqlCommand("sp_LoginKhachHang", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(paramUsername);
                            command.Parameters.Add(paramCurrentPassword);

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    // Mật khẩu hiện tại chính xác, tiến hành cập nhật mật khẩu mới
                                    var paramMaNguoiDung = new SqlParameter("@MaNguoiDung", maNguoiDung);
                                    var paramNewPassword = new SqlParameter("@Password", newPassword);

                                    await _context.Database.ExecuteSqlRawAsync(
                                        "EXEC sp_DoiMatKhau @MaNguoiDung, @Password",
                                        paramMaNguoiDung, paramNewPassword);

                                    ViewBag.Message = "Đổi mật khẩu thành công.";
                                    return RedirectToAction("Profile");
                                }
                                else
                                {
                                    ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng.");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đổi mật khẩu.");
                }
            }
            return View();
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "UserDH");
        }

        private int GetMaKhachHang(int maNguoiDung)
        {
            using (var conn = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT MaKhachHang FROM KhachHang WHERE MaNguoiDung = @MaNguoiDung", conn))
                {
                    cmd.Parameters.AddWithValue("@MaNguoiDung", maNguoiDung);

                    var result = cmd.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int maKhachHang))
                    {
                        return maKhachHang;
                    }
                }
            }
            return -1; 
        }

    }
}
