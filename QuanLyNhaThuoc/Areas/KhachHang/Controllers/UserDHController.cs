
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
                    // Tạo các tham số cho stored procedure
                    var paramUsername = new SqlParameter("@Username", username);
                    var paramPassword = new SqlParameter("@Password", password);

                    // Kết nối cơ sở dữ liệu và thực thi stored procedure
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
                                    int maNguoiDung = reader.GetInt32(0); // Lấy giá trị MaNguoiDung
                                    int? maVaiTro = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1); // Lấy giá trị MaVaiTro (có thể null)

                                    // Kiểm tra thông tin đăng nhập
                                    if (maNguoiDung > 0 && maVaiTro == 3) // Đảm bảo vai trò là khách hàng
                                    {
                                        // Tạo Claims để lưu thông tin đăng nhập
                                        var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name, username),
                                    new Claim("UserId", maNguoiDung.ToString()), // Lưu mã người dùng trong claim
                                    new Claim("Role", maVaiTro.ToString()) // Lưu vai trò người dùng trong claim
                                };

                                        // Tạo ClaimsIdentity và ClaimsPrincipal cho phiên đăng nhập
                                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                                        // Đăng nhập với ClaimsPrincipal và lưu vào phiên
                                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                                        // Đăng nhập thành công, chuyển hướng đến trang Profile
                                        return RedirectToAction("Profile");
                                    }
                                }
                            }
                        }
                    }

                    // Nếu không tìm thấy người dùng hoặc tài khoản không hoạt động
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng hoặc tài khoản không hoạt động.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình đăng nhập.");
                }
            }

            // Nếu có lỗi, hiển thị lại form đăng nhập
            return View();
        }


    }
}
