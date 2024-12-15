using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace QuanLyNhaThuoc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang/[controller]")]
    public class LichSuDonHangController : Controller
    {
        private readonly QL_NhaThuocContext _context;

        public LichSuDonHangController(QL_NhaThuocContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string statusFilter = "all")
        {
            // Lấy MaKhachHang từ Claims
            var maKhachHangClaim = User.Claims.FirstOrDefault(c => c.Type == "MaKhachHang");
            if (maKhachHangClaim == null)
            {
                return RedirectToAction("Login", "UserDH"); 
            }

            int maKhachHang = int.Parse(maKhachHangClaim.Value);
            var query = _context.LichSuDonHang
                .Where(ldh => ldh.MaKhachHang == maKhachHang)  
                .OrderByDescending(ldh => ldh.NgayDatHang)  
                .AsQueryable();

            // Lọc theo trạng thái nếu có
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "all")
            {
                query = query.Where(ldh => ldh.TrangThai == statusFilter);
            }

            // Lấy danh sách đơn hàng
            var lichSuDonHang = await query.ToListAsync();

            // Trả về view với dữ liệu đơn hàng
            return View(lichSuDonHang);
        }
    }
}
