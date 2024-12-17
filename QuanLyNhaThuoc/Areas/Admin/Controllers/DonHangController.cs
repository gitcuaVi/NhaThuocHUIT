using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class DonHangController : Controller
    {
        private readonly QL_NhaThuocContext _context;
        private readonly ILogger<DonHangController> _logger;

        public DonHangController(QL_NhaThuocContext context, ILogger<DonHangController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString, DateTime? startDate, DateTime? endDate, string statusFilter, string searchnv)
        {
            // Lấy danh sách đơn hàng kèm thông tin khách hàng
            var query = _context.DonHangs
                .Include(dh => dh.KhachHang)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(d => d.MaDonHang.ToString().Contains(searchString) ||
                                         d.KhachHang.TenKhachHang.Contains(searchString));
            }
            if (startDate.HasValue)
            {
                query = query.Where(d => d.NgayDatHang >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(d => d.NgayDatHang <= endDate.Value);
            }
            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(d => d.TrangThai == statusFilter);
            }
            if (!string.IsNullOrEmpty(searchnv))
            {
                query = query.Where(d => d.MaNhanVien.ToString().Contains(searchnv));
            }

            var donHangs = await query
                .Select(d => new DonHangKhachHangViewModels
                {
                    MaDonHang = d.MaDonHang,
                    TongTien = d.TongTien,
                    GhiChu = d.GhiChu,
                    NgayDatHang = d.NgayDatHang,
                    NgayCapNhat = d.NgayCapNhat,
                    DiaChiDonHang = d.DiaChi,
                    NgayGiaoHang = d.NgayGiaoHang,
                    TrangThai = d.TrangThai,
                    MaNhanVien = d.MaNhanVien,
                    MaKhachHang = d.MaKhachHang,
                    TenKhachHang = d.KhachHang.TenKhachHang,
                    GioiTinh = d.KhachHang.GioiTinh,
                    DiaChiKhachHang = d.KhachHang.DiaChi,
                    NgaySinh = d.KhachHang.NgaySinh,
                    SoDienThoai = d.KhachHang.SoDienThoai
                })
                .ToListAsync();

            return View(donHangs);
        }

        // GET: admin/DonHang/Update/5
        [HttpGet("Update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null)
            {
                return NotFound();
            }

            var viewModel = new UpdateDonHang
            {
                MaDonHang = donHang.MaDonHang,
                TrangThai = donHang.TrangThai
            };

            return View(viewModel);
        }

        // POST: admin/DonHang/Update
        [HttpPost("Update/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateDonHang model)
        {
            if (ModelState.IsValid)
            {
                // Lấy mã nhân viên từ claims
                var maNhanVienClaim = User.Claims.FirstOrDefault(c => c.Type == "MaNhanVien")?.Value;
                if (maNhanVienClaim == null)
                {
                    ViewBag.ErrorMessage = "Không tìm thấy mã nhân viên trong phiên đăng nhập.";
                    return View(model);
                }

                try
                {
                    // Gọi stored procedure để cập nhật đơn hàng
                    var maNhanVien = int.Parse(maNhanVienClaim);
                    var parameters = new[]
                    {
                        new SqlParameter("@MaDonHang", model.MaDonHang),
                        new SqlParameter("@TrangThai", model.TrangThai),
                        new SqlParameter("@MaNhanVien", maNhanVien)
                    };

                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC CapNhatTrangThaiVaMaNhanVien @MaDonHang, @TrangThai, @MaNhanVien", parameters);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật đơn hàng");
                    ViewBag.ErrorMessage = "Đã xảy ra lỗi khi cập nhật đơn hàng.";
                }
            }

            return View(model);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var donHang = await _context.DonHangs
                .FirstOrDefaultAsync(dh => dh.MaDonHang == id);

            if (donHang == null)
            {
                return NotFound();
            }

            var chiTietDonHangs = await _context.ChiTietDonHangs
                .Where(ct => ct.MaDonHang == id)
                .Select(ct => new ChiTietDonHangViewModel
                {
                    MaThuoc = ct.MaThuoc,
                    SoLuong = ct.SoLuong,

                    Gia = ct.Gia,
                    ThanhTien = ct.SoLuong * ct.Gia,
                    TenThuoc = _context.Thuocs
                        .Where(t => t.MaThuoc == ct.MaThuoc)
                        .Select(t => t.TenThuoc)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var viewModel = new DonHangDetailsViewModel
            {
                MaDonHang = donHang.MaDonHang,
                NgayDatHang = donHang.NgayDatHang,
                DiaChi = donHang.DiaChi,
                TrangThai = donHang.TrangThai,
                TongTien = donHang.TongTien,
                ChiTietDonHangs = chiTietDonHangs
            };

            return View(viewModel);
        }
    }
}