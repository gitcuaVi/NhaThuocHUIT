using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class SaoLuuVaPhucHoiController : Controller
    {
        private readonly QL_NhaThuocContext _context;

        public SaoLuuVaPhucHoiController(QL_NhaThuocContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var backups = await _context.SaoLuuVaPhucHois.ToListAsync();
            return View(backups);
        }
        [HttpPost("SaoLuu")]
        public async Task<IActionResult> SaoLuu(string backupLocation)
        {
             backupLocation = "C:\\Program Files\\Microsoft SQL Server\\MSSQL16.MSSQLSERVER\\MSSQL\\Backup";

            try
            {
                string backupFileName = $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                string backupFilePath = Path.Combine(backupLocation, backupFileName);

                // Kết nối với cơ sở dữ liệu
                var connectionString = _context.Database.GetConnectionString();

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string backupQuery = $"BACKUP DATABASE [QL_NhaThuoc] TO DISK = '{backupFilePath}'";
                    using (var command = new SqlCommand(backupQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }

                // Lưu bản ghi sao lưu vào cơ sở dữ liệu
                var backupRecord = new SaoLuuVaPhucHoi
                {
                    MaNhanVien = GetMaNhanVienFromClaims(), // Lấy mã nhân viên từ claims
                    ThoiGianSaoLuu = DateTime.Now,
                    TrangThaiSaoLuu = "Thành công",
                    DiaChi = backupFilePath,
                    TenFileSaoLuu = backupFileName
                };
                _context.SaoLuuVaPhucHois.Add(backupRecord);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Sao lưu thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Đã xảy ra lỗi khi sao lưu: {ex.Message}. StackTrace: {ex.StackTrace}";
                return RedirectToAction("Index");
            }
        }


        // Hàm để lấy MaNhanVien từ Claims
        private int GetMaNhanVienFromClaims()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int maNguoiDung))
            {
                return _context.NhanViens
                    .Where(nv => nv.MaNguoiDung == maNguoiDung)
                    .Select(nv => nv.MaNhanVien)
                    .FirstOrDefault();
            }
            return -1; // Trả về -1 nếu không tìm thấy
        }
        [HttpPost("PhucHoi")]
        public async Task<IActionResult> PhucHoi(string backupFile)
        {
            if (string.IsNullOrEmpty(backupFile))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng chỉ định tệp sao lưu.");
                return View("Index");
            }
            
            try
            {
              
                // Kết nối với cơ sở dữ liệu
                var connectionString = _context.Database.GetConnectionString();

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Chuyển sang cơ sở dữ liệu master
                    string switchDatabaseQuery = "USE master";
                    using (var command = new SqlCommand(switchDatabaseQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    // Đặt cơ sở dữ liệu vào trạng thái SINGLE_USER để ngắt kết nối
                    string setSingleUserQuery = "ALTER DATABASE [QL_NhaThuoc] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    using (var command = new SqlCommand(setSingleUserQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    // Thực hiện phục hồi
                    string restoreQuery = $"RESTORE DATABASE [QL_NhaThuoc] FROM DISK = '{backupFile}' WITH REPLACE";
                    using (var command = new SqlCommand(restoreQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    // Đặt cơ sở dữ liệu trở lại chế độ MULTI_USER sau khi phục hồi xong
                    string setMultiUserQuery = "ALTER DATABASE [QL_NhaThuoc] SET MULTI_USER";
                    using (var command = new SqlCommand(setMultiUserQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }


                // Lưu thông tin phục hồi vào cơ sở dữ liệu
                var restoreRecord = new SaoLuuVaPhucHoi
                {
                    MaNhanVien = GetMaNhanVienFromClaims(),
                    ThoiGianPhucHoi = DateTime.Now,
                    TrangThaiSaoLuu = "Thành công",
                    DiaChi = backupFile,
                    TenFileSaoLuu = Path.GetFileName(backupFile)
                };
                _context.SaoLuuVaPhucHois.Add(restoreRecord);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Phục hồi thành công!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }

}
