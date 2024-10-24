using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;
using Microsoft.Extensions.Logging;


namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
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
        public async Task<IActionResult> Index(string searchString, DateTime? startDate, DateTime? endDate, string statusFilter)
        {
            // Lấy danh sách đơn hàng kèm thông tin khách hàng từ view
            var query = _context.DonHangs
                .Include(dh => dh.KhachHang) // Giả sử DonHang có liên kết với KhachHang
                .AsQueryable();

            // Lọc theo từ khóa tìm kiếm (Tên khách hàng hoặc mã đơn hàng)
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(d => d.MaDonHang.ToString().Contains(searchString) || d.KhachHang.TenKhachHang.Contains(searchString));
            }

            // Lọc theo khoảng thời gian (Ngày bắt đầu và ngày kết thúc)
            if (startDate.HasValue)
            {
                query = query.Where(d => d.NgayDatHang >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(d => d.NgayDatHang <= endDate.Value);
            }

            // Lọc theo trạng thái đơn hàng
            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(d => d.TrangThai == statusFilter);
            }

            // Lấy danh sách đơn hàng sau khi đã lọc
            var donHangs = await query.Select(d => new DonHangKhachHangViewModels
            {
                MaDonHang = d.MaDonHang,
                TongTien = d.TongTien,
                GhiChu = d.GhiChu,
                NgayDatHang = d.NgayDatHang,
                NgayCapNhat = d.NgayCapNhat,
                DiaChiDonHang = d.DiaChi,
                NgayGiaoHang = d.NgayGiaoHang,
                TrangThai = d.TrangThai,
                MaKhachHang = d.MaKhachHang,
                TenKhachHang = d.KhachHang.TenKhachHang,
                GioiTinh = d.KhachHang.GioiTinh,
                DiaChiKhachHang = d.KhachHang.DiaChi,
                NgaySinh = d.KhachHang.NgaySinh,
                SoDienThoai = d.KhachHang.SoDienThoai
            }).ToListAsync();

            // Trả về view với danh sách đơn hàng đã được lọc
            return View(donHangs);
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách đơn hàng kèm theo thông tin khách hàng từ view
            var donHangs = await _context.DonHangKhachHangViewModels
                                          .FromSqlRaw("SELECT * FROM vw_DonHang_KhachHang")
                                          .ToListAsync();

            // Trả về view với đúng kiểu dữ liệu
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

            // Trả về view cập nhật với model là đối tượng đơn hàng
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
                var donHang = await _context.DonHangs.FindAsync(model.MaDonHang);
                if (donHang == null)
                {
                    return NotFound();
                }

                // Cập nhật trạng thái đơn hàng
                donHang.TrangThai = model.TrangThai;

                // Lưu thay đổi vào database
                _context.Update(donHang);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // Quay về danh sách đơn hàng
            }

            return View(model); // Trả lại view nếu có lỗi trong model
        }
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            // Tìm đơn hàng theo mã đơn hàng
            var donHang = await _context.DonHangs
                .FirstOrDefaultAsync(dh => dh.MaDonHang == id);

            // Kiểm tra nếu không tìm thấy đơn hàng
            if (donHang == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy đơn hàng
            }

            // Lấy danh sách chi tiết đơn hàng theo mã đơn hàng
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
                        .FirstOrDefault() // Lấy tên thuốc cho mỗi chi tiết
                }).ToListAsync();

            // Tạo view model chứa thông tin cần hiển thị trong view chi tiết
            var viewModel = new DonHangDetailsViewModel
            {
                MaDonHang = donHang.MaDonHang,
                NgayDatHang = donHang.NgayDatHang,
                DiaChi = donHang.DiaChi,
                TrangThai = donHang.TrangThai,
                TongTien = donHang.TongTien,
                ChiTietDonHangs = chiTietDonHangs // Gán danh sách chi tiết đơn hàng
            };

            // Trả về view với model là đối tượng đơn hàng và chi tiết đơn hàng
            return View(viewModel);
        }



    }

}
