using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using QuanLyNhaThuoc.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly QL_NhaThuocContext _context;
        public AdminController(QL_NhaThuocContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    connection.Open();
                    var query = @"
                SELECT TOP 1 * 
                FROM NguoiDung
                WHERE (Email = @Username OR SoDienThoai = @Username)
                AND Password = dbo.HashPassword(@Password)
                AND TrangThai = 'Active'";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", model.Username);
                        command.Parameters.AddWithValue("@Password", model.Password);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var userRole = reader["MaVaiTro"];
                                var userId = reader["MaNguoiDung"];

                                if (Convert.ToInt32(userRole) == 1) // Admin Role
                                {
                                    HttpContext.Session.SetString("UserRole", "Admin");
                                    HttpContext.Session.SetInt32("UserId", Convert.ToInt32(userId));
                                    var sessionRole = HttpContext.Session.GetString("UserRole");
                                    var sessionUserId = HttpContext.Session.GetInt32("UserId");
                                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                                }
                                else if (Convert.ToInt32(userRole) == 2) // Employee Role
                                {
                                    HttpContext.Session.SetString("UserRole", "NhanVien");
                                    HttpContext.Session.SetInt32("UserId", Convert.ToInt32(userId));
                                    var sessionRole = HttpContext.Session.GetString("UserRole");
                                    var sessionUserId = HttpContext.Session.GetInt32("UserId");
                                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                                }
                            }
                        }
                    }

                    ViewBag.ErrorMessage = "Invalid email or password";
                }
            }

            return View(model);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Admin");
        }


    }
}
