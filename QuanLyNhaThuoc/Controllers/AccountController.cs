using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using QuanLyNhaThuoc.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Models;


namespace QuanLyNhaThuoc.Controllers
{
    public class AccountController : Controller
    {
        private readonly QL_NhaThuocContext _context;

        public AccountController(QL_NhaThuocContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            using (var conn = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_LoginNguoiDung", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int maNguoiDung = reader.IsDBNull(0) ? -1 : reader.GetInt32(0);
                            int? maVaiTro = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1);

                            if (maNguoiDung != -1)
                            {
                                // Tạo claims và lưu vào phiên đăng nhập
                                var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name, username),
                                    new Claim("UserId", maNguoiDung.ToString()), // Lưu mã người dùng trong claim
                                };

                                // Chỉ lấy MaNhanVien nếu không phải là Admin
                                if (maVaiTro == 2) // Nhân viên
                                {
                                    int maNhanVien = GetMaNhanVien(maNguoiDung);
                                    if (maNhanVien == -1)
                                    {
                                        ViewBag.ErrorMessage = "Không tìm thấy mã nhân viên tương ứng.";
                                        return View();
                                    }
                                    claims.Add(new Claim("MaNhanVien", maNhanVien.ToString())); // Lưu mã nhân viên trong claim
                                }

                                // Lưu quyền người dùng
                                if (maVaiTro == 1) // Admin
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                                }
                                else if (maVaiTro == 2) // Nhân viên
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "NhanVien"));
                                }

                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                {
                                    return Redirect(returnUrl);
                                }
                                if (maVaiTro == 1) // Admin
                                {
                                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                                }
                                else if (maVaiTro == 2) // Nhân viên
                                {
                                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                                }
                            }
                            else
                            {
                                ViewBag.ErrorMessage = "Thông tin đăng nhập không hợp lệ hoặc tài khoản không hoạt động.";
                                return View();
                            }
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Đăng nhập thất bại. Vui lòng thử lại.";
                            return View();
                        }
                    }
                }
            }

            ViewBag.ErrorMessage = "Lỗi không xác định. Vui lòng thử lại.";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // Hàm để lấy MaNhanVien dựa vào MaNguoiDung
        private int GetMaNhanVien(int maNguoiDung)
        {
            using (var conn = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT MaNhanVien FROM NhanVien WHERE MaNguoiDung = @MaNguoiDung", conn))
                {
                    cmd.Parameters.AddWithValue("@MaNguoiDung", maNguoiDung);

                    var result = cmd.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int maNhanVien))
                    {
                        return maNhanVien;
                    }
                }
            }
            return -1; // không tìm thấy
        }



    }
}