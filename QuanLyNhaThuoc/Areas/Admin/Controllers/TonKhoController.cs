using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class TonKhoController : Controller
    {
        private readonly QL_NhaThuocContext _context;

        public TonKhoController(QL_NhaThuocContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 10)
        {
            // Lấy danh sách tồn kho từ database
            var tonKhosQuery = _context.TonKhos
                .Include(tk => tk.MaThuocNavigation)
                .AsNoTracking();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                tonKhosQuery = tonKhosQuery.Where(tk =>
                    tk.MaThuocNavigation.TenThuoc.Contains(searchString) ||
                    tk.MaTonKho.ToString().Contains(searchString));
            }

            // Tổng số bản ghi
            int totalItems = await tonKhosQuery.CountAsync();

            // Phân trang
            var tonKhos = await tonKhosQuery
                .OrderBy(tk => tk.MaTonKho) // Sắp xếp nếu cần
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Xử lý cảnh báo
            foreach (var item in tonKhos)
            {
                item.WarningMessage = null;
                if (item.SoLuongTon < item.SoLuongCanhBao)
                {
                    item.WarningMessage = $"Số lượng tồn kho của {item.MaThuocNavigation.TenThuoc} dưới mức cảnh báo!";
                }
                else if (item.SoLuongTon > item.SoLuongToiDa)
                {
                    item.WarningMessage = $"Số lượng tồn kho của {item.MaThuocNavigation.TenThuoc} vượt quá giới hạn tối đa!";
                }
            }

            // Truyền dữ liệu phân trang và tìm kiếm
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentFilter = searchString;

            return View(tonKhos);
        }


        [HttpPost("CapNhatSoLuongTonKho")]
        public async Task<IActionResult> CapNhatSoLuongTonKho(int maTonKho, int? SoLuongCanhBao, int? SoLuongToiDa)
        {
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@MaTonKho", maTonKho),
            new SqlParameter("@SoLuongCanhBao", (object?)SoLuongCanhBao ?? DBNull.Value),
            new SqlParameter("@SoLuongToiDa", (object?)SoLuongToiDa ?? DBNull.Value)
        };

                await _context.Database.ExecuteSqlRawAsync("EXEC sp_CapNhatSoLuongTonKho @MaTonKho, @SoLuongCanhBao, @SoLuongToiDa", parameters);

                return Json(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi khi cập nhật: {ex.Message}" });
            }
        }



    }
}
