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
        public IActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var phieuNhapQuery = _context.PhieuNhaps
                .Include(p => p.MaNhanVienNavigation)
                .Include(p => p.ChiTietPns)
                    .ThenInclude(ctpn => ctpn.MaTonKhoNavigation)
                .AsQueryable();
            // Lọc danh sách phiếu nhập
            if (!string.IsNullOrEmpty(searchString))
            {
                phieuNhapQuery = phieuNhapQuery.Where(pn =>
                    pn.MaPhieuNhap.ToString().Contains(searchString) || 
                    pn.MaNhanVienNavigation.Ten.Contains(searchString) ||
                    pn.NhaCungCap.Contains(searchString));
            }

            int totalItems = phieuNhapQuery.Count();

            // Phân trang, sắp xếp theo ngày giảm dần
            var phieuNhaps = phieuNhapQuery
                .OrderByDescending(pn => pn.NgayNhap)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentFilter = searchString;

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
                //ngay hien tai
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

                TempData["SuccessMessage"] = "Phiếu nhập đã được tạo thành công.";
                return RedirectToAction("Index", new { maNhanVien, ngayNhap });
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi khi tạo phiếu nhập: {ex.Message}");
            }
        }


        [HttpGet("ChiTiet/{id}")]
        public IActionResult ChiTiet(int id)
        {
            //chi tiet theo phieunhap
            var chiTietPhieuNhap = _context.ChiTietPns
                .Include(ct => ct.MaThuocNavigation)
                .Include(ct => ct.MaTonKhoNavigation)
                .Where(ct => ct.MaPhieuNhap == id)
                .ToList();

            if (chiTietPhieuNhap == null || !chiTietPhieuNhap.Any())
            {
                TempData["ErrorMessage"] = "Không tìm thấy chi tiết phiếu nhập.";
                return RedirectToAction("Index");
            }

            ViewBag.MaPhieuNhap = id;
            return View(chiTietPhieuNhap);
        }

        [HttpPost("ThucHien")]
        public IActionResult ThucHien(int maPhieuNhap, [FromForm] Dictionary<string, string> trangThai)
        {
            try
            {
                //lap trang thai
                foreach (var key in trangThai.Keys)
                {
                    if (key.StartsWith("TrangThai_"))
                    {
                        var maChiTietPn = int.Parse(key.Replace("TrangThai_", ""));
                        var trangThaiValue = bool.Parse(trangThai[key]);

                        // Lấy phiếu nhập hiện tại
                        var chiTietPn = _context.ChiTietPns.Find(maChiTietPn);

                        if (chiTietPn == null || chiTietPn.TrangThai != null)
                        {
                            TempData["ErrorMessage"] = $"Chi tiết phiếu nhập {maChiTietPn} đã được cập nhật trước đó.";
                            continue;
                        }

                        // Gọi stored procedure
                        var parameters = new[]
                        {
                    new SqlParameter("@MaChiTietPN", maChiTietPn),
                    new SqlParameter("@TrangThai", trangThaiValue)
                };

                        _context.Database.ExecuteSqlRaw("EXEC sp_CapNhatTrangThaiChiTietPN @MaChiTietPN, @TrangThai", parameters);
                    }
                }

                TempData["SuccessMessage"] = "Cập nhật trạng thái thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
            }

            return RedirectToAction("ChiTiet", new { id = maPhieuNhap });
        }


    }
}
