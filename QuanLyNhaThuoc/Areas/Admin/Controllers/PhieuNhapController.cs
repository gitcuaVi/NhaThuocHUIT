using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;
using System.Data;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
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
            // Lấy danh sách nhân viên
            ViewBag.NhanViens = _context.NhanViens.Select(nv => new { nv.MaNhanVien, nv.HoTen }).ToList();

            // Lấy danh sách thuốc
            ViewBag.Thuocs = _context.Thuocs.Select(t => new { t.MaThuoc, t.TenThuoc }).ToList();

            return View(); // Hiển thị view chứa form tạo phiếu nhập
        }

        [HttpPost("create")]
        public IActionResult CreatePhieuNhap(int maNhanVien, decimal tongTien, string ghiChu, string nhaCungCap, List<ChiTietPn> chiTietPns)
        {
            try
            {
                // Lấy ngày hiện tại
                DateTime ngayNhap = DateTime.Now;

                // Lấy tên nhân viên dựa trên mã nhân viên
                var nhanVien = _context.NhanViens
                    .Where(nv => nv.MaNhanVien == maNhanVien)
                    .Select(nv => nv.HoTen)
                    .FirstOrDefault();

                if (nhanVien == null)
                {
                    return BadRequest("Mã nhân viên không hợp lệ.");
                }

                // Chuẩn bị dữ liệu cho bảng ChiTietPn
                var chiTietPnTable = new DataTable();
                chiTietPnTable.Columns.Add("MaThuoc", typeof(int));
                chiTietPnTable.Columns.Add("SoLuong", typeof(int));
                chiTietPnTable.Columns.Add("DonGiaXuat", typeof(decimal));
                chiTietPnTable.Columns.Add("MaTonKho", typeof(int));

                foreach (var chiTiet in chiTietPns)
                {
                    // Lấy tên thuốc dựa trên mã thuốc
                    var tenThuoc = _context.Thuocs
                        .Where(t => t.MaThuoc == chiTiet.MaThuoc)
                        .Select(t => t.TenThuoc)
                        .FirstOrDefault();

                    if (tenThuoc == null)
                    {
                        return BadRequest("Mã thuốc không hợp lệ.");
                    }

                    chiTietPnTable.Rows.Add(chiTiet.MaThuoc, chiTiet.SoLuong, chiTiet.DonGiaXuat, chiTiet.MaTonKho);
                }

                // Gọi stored procedure
                var parameters = new[]
                {
                new SqlParameter("@MaNhanVien", maNhanVien),
                new SqlParameter("@TongTien", tongTien != 0 ? tongTien : (object)DBNull.Value),
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
