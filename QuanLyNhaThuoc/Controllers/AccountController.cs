using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using QuanLyNhaThuoc.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
            ViewData["ReturnUrl"] = returnUrl; // Pass the returnUrl to the view
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
                                // Create claims and sign in the user
                                var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, username),
                            new Claim("UserId", maNguoiDung.ToString())
                        };

                                if (maVaiTro == 1) // Admin
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                                }
                                else if (maVaiTro == 2) // Employee
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "Employee"));
                                }

                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                                // If returnUrl is not null or empty, redirect to that page
                                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                {
                                    return Redirect(returnUrl); // Redirect to original page before login
                                }

                                // Otherwise, redirect based on the user's role
                                if (maVaiTro == 1) // Admin
                                {
                                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                                }
                                else if (maVaiTro == 2) // Employee
                                {
                                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                                }
                            }
                            else
                            {
                                ViewBag.ErrorMessage = "Invalid credentials or inactive account.";
                                return View();
                            }
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Login failed. Please try again.";
                            return View();
                        }
                    }
                }
            }

            ViewBag.ErrorMessage = "Unexpected error occurred. Please try again.";
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
