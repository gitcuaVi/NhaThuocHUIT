using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Security.Claims;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "NhanVien")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class ChamCongController : Controller
    {
        private readonly QL_NhaThuocContext db;

        public ChamCongController(QL_NhaThuocContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Index(int? month, int? year, int page = 1, int pageSize = 10)
        {
            // Lấy mã nhân viên từ claims
            var maNhanVien = GetMaNhanVienFromClaims();
            if (maNhanVien == -1)
            {
                return RedirectToAction("Index", "Home");
            }
            var query = db.ChamCongs.AsQueryable().Where(cc => cc.MaNhanVien == maNhanVien);
            if (month.HasValue && year.HasValue)
            {
                query = query.Where(cc => cc.NgayChamCong.Month == month && cc.NgayChamCong.Year == year);
            }

            // Phân trang
            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var chamCongList = query
                .OrderByDescending(cc => cc.NgayChamCong)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Trả dữ liệu sang view
            ViewBag.CurrentMonth = month;
            ViewBag.CurrentYear = year;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(chamCongList);
        }

        // Phương thức chấm công vào

        [Route("ChamCongVao")]
        [HttpPost]
        public IActionResult ChamCongVao()
        {
            var maNhanVien = GetMaNhanVienFromClaims();
            if (maNhanVien == -1)
            {
                return Json(new { success = false, message = "Không tìm thấy mã nhân viên." });
            }

            using (var conn = new SqlConnection(db.Database.GetDbConnection().ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_ThemChamCongVao", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaNhanVien", maNhanVien);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        return Json(new { success = true, message = "Chấm công vào thành công. MaNhanVien: " + maNhanVien });
                    }
                    catch (SqlException ex)
                    {
                        return Json(new { success = false, message = ex.Message });
                    }
                }
            }
        }


        // Phương thức chấm công ra
        [Route("ChamCongRa")]
        [HttpPost]
        public IActionResult ChamCongRa()
        {
            var maNhanVien = GetMaNhanVienFromClaims();
            if (maNhanVien == -1)
            {
                return Json(new { success = false, message = "Không tìm thấy mã nhân viên." });
            }

            using (var conn = new SqlConnection(db.Database.GetDbConnection().ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_ChamCongRa", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaNhanVien", maNhanVien);

                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var message = reader["ThongBao"].ToString();
                                return Json(new { success = true, message = "Chấm công ra thành công. MaNhanVien: " + maNhanVien });
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        return Json(new { success = false, message = ex.Message });
                    }
                }
            }

            return Json(new { success = false, message = "Chấm công ra không thành công." });
        }

        // Hàm để lấy mã nhân viên từ Claims
        private int GetMaNhanVienFromClaims()
        {
            if (User.IsInRole("NhanVien"))
            {
                return int.Parse(User.FindFirstValue("MaNhanVien"));
            }
            return -1; // Không phải nhân viên
        }
    }
}
