using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;
using System.Threading.Tasks;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
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
        public async Task<IActionResult> Index()
        {
            var tonKhos = await _context.TonKhos
                .Include(tk => tk.MaThuocNavigation)
                .AsNoTracking()
                .ToListAsync();

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
