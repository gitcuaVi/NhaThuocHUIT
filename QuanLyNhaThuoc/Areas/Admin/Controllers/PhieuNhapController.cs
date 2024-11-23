using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;
using System.Data;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class PhieuNhapController : Controller
    {
        private readonly QL_NhaThuocContext _context;

        public PhieuNhapController(QL_NhaThuocContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var phieuNhaps = _context.PhieuNhaps
                .Include(p => p.MaNhanVienNavigation)
                .Include(p => p.ChiTietPns)
                    .ThenInclude(ctpn => ctpn.MaTonKhoNavigation)
                .ToList();

            return View(phieuNhaps);
        }
        [HttpGet("create")]
        public IActionResult CreatePhieuNhap()
        {
            ViewBag.NhanViens = _context.NhanViens.Select(nv => new { nv.MaNhanVien, nv.HoTen }).ToList();
            ViewBag.Thuocs = _context.Thuocs.Select(t => new { t.MaThuoc, t.TenThuoc }).ToList();
            return View();
        }

        [HttpPost("create")]
        public IActionResult CreatePhieuNhap(int maNhanVien, string ghiChu, string nhaCungCap, List<ChiTietPn> chiTietPns)
        {
            try
            {
                DateTime ngayNhap = DateTime.Now;
                var chiTietPnTable = new DataTable();
                chiTietPnTable.Columns.Add("MaThuoc", typeof(int));
                chiTietPnTable.Columns.Add("SoLuong", typeof(int));
                chiTietPnTable.Columns.Add("DonGiaXuat", typeof(decimal));
                chiTietPnTable.Columns.Add("MaTonKho", typeof(int)); 

                decimal tongTien = 0;

                foreach (var chiTiet in chiTietPns)
                {
                    var thuoc = _context.Thuocs.Find(chiTiet.MaThuoc);
                    if (thuoc == null)
                    {
                        return BadRequest("Mã thuốc không hợp lệ.");
                    }

                    chiTietPnTable.Rows.Add(chiTiet.MaThuoc, chiTiet.SoLuong, chiTiet.DonGiaXuat, chiTiet.MaTonKho);
                    tongTien += chiTiet.SoLuong * chiTiet.DonGiaXuat;
                }


                var parameters = new[]
                {
                    new SqlParameter("@MaNhanVien", maNhanVien),
                    new SqlParameter("@TongTien", tongTien),
                    new SqlParameter("@NgayNhap", ngayNhap),
                    new SqlParameter("@GhiChu", ghiChu ?? (object)DBNull.Value),
                    new SqlParameter("@NhaCungCap", nhaCungCap),
                    new SqlParameter("@ChiTietPns", SqlDbType.Structured)
                    {
                        TypeName = "dbo.ChiTietPnType",
                        Value = chiTietPnTable
                    }
                };

                _context.Database.ExecuteSqlRaw("EXEC dbo.sp_InsertPhieuNhap @MaNhanVien, @TongTien, @NgayNhap, @GhiChu, @NhaCungCap, @ChiTietPns", parameters);

                return Ok("Phiếu nhập đã được tạo thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi khi tạo phiếu nhập: {ex.Message}");
            }
        }


    }
}
