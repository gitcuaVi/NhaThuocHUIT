using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.Models;
using QuanLyNhaThuoc.ViewModels;
using System.Data;
using System.Security.Claims;
using KhachHangModel = QuanLyNhaThuoc.Models.KhachHang;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class NhanVienHoaDonController : Controller
    {

        private readonly QL_NhaThuocContext db;

        public NhanVienHoaDonController(QL_NhaThuocContext context)
        {
            db = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId)
        {
            // Lấy danh sách danh mục
            var danhMucList = await db.DanhMucs.ToListAsync();
            ViewBag.DanhMucList = danhMucList;

            // Nếu không có categoryId, hiển thị trang mặc định
            if (categoryId == null)
            {
                return View(new List<SanPhamViewModel>());
            }

            // Lấy danh sách thuốc theo danh mục
            var thuocList = await db.Thuocs
                .Where(t => t.MaLoaiSanPhamNavigation.MaDanhMuc == categoryId)
                .Include(t => t.HinhAnhs)
                .Select(t => new SanPhamViewModel
                {
                    MaThuoc = t.MaThuoc,
                    TenThuoc = t.TenThuoc,
                    HinhAnhDauTien = t.HinhAnhs.FirstOrDefault().UrlAnh,
                    DonGia = t.DonGia,
                    DonVi = t.DonVi
                })
                .ToListAsync();

            return View(thuocList);
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(int maThuoc, int soLuong)
        {
            try
            {
                int maGioHang = 0;

                // Lấy mã giỏ hàng từ session hoặc tạo mới
                if (HttpContext.Session.GetInt32("MaGioHang") == null)
                {
                    var gioHang = new GioHang { TongTien = 0, MaKhachHang = null };
                    db.GioHangs.Add(gioHang);
                    await db.SaveChangesAsync();
                    maGioHang = gioHang.MaGioHang;

                    HttpContext.Session.SetInt32("MaGioHang", maGioHang);
                }
                else
                {
                    maGioHang = HttpContext.Session.GetInt32("MaGioHang").Value;
                }

                // Gọi stored procedure
                await db.Database.ExecuteSqlRawAsync("EXEC sp_AddToCartNhanVienThanhToan @MaGioHang, @MaThuoc, @SoLuong",
                    new SqlParameter("@MaGioHang", maGioHang),
                    new SqlParameter("@MaThuoc", maThuoc),
                    new SqlParameter("@SoLuong", soLuong));

                return Json(new { success = true, message = "Thêm vào giỏ hàng thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [HttpGet("Cart")]
        public async Task<IActionResult> Cart()
        {
            try
            {
                // Lấy mã giỏ hàng từ session
                var maGioHang = HttpContext.Session.GetInt32("MaGioHang");
                if (maGioHang == null)
                {
                    return View(new List<GioHangNhanVienThanhToanView>());
                }

                // Gọi stored procedure và nhận kết quả
                var gioHang = await db.GioHangNhanVienThanhToanViews
                    .FromSqlRaw("EXEC sp_GetCartNhanVienThanhToan @MaGioHang", new SqlParameter("@MaGioHang", maGioHang))
                    .ToListAsync();

                return View(gioHang);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(new List<GioHangNhanVienThanhToanView>());
            }
        }



        private int GetMaNhanVienFromClaims()
        {
            if (User.IsInRole("NhanVien"))
            {
                return int.Parse(User.FindFirstValue("MaNhanVien"));
            }
            return -1; // Không phải nhân viên
        }
        [HttpPost("CreateCustomerAndOrder")]
        public async Task<IActionResult> CreateCustomerAndOrder(
            string TenKhachHang,
            string GioiTinh,
            string DiaChi,
            DateTime? NgaySinh,
            string SoDienThoai,
            int? MaNguoiDung,
            int Diem,
            string GhiChu)
        {
            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                // Tạo khách hàng
                var parametersCustomer = new[]
                {
            new SqlParameter("@TenKhachHang", TenKhachHang ?? string.Empty),
            new SqlParameter("@GioiTinh", GioiTinh ?? string.Empty),
            new SqlParameter("@DiaChi", DiaChi ?? "Chưa cập nhật"),
            new SqlParameter("@NgaySinh", NgaySinh ?? DateTime.Parse("1900-01-01")),
            new SqlParameter("@SoDienThoai", SoDienThoai ?? string.Empty),
            new SqlParameter("@MaNguoiDung", (object)MaNguoiDung ?? DBNull.Value),
            new SqlParameter("@Diem", Diem)
        };

                await db.Database.ExecuteSqlRawAsync(
                    "EXEC sp_CreateCustomer @TenKhachHang, @GioiTinh, @DiaChi, @NgaySinh, @SoDienThoai, @MaNguoiDung, @Diem",
                    parametersCustomer
                );

                // Lấy mã khách hàng vừa tạo
                var maKhachHang = await db.KhachHangs
                    .Where(kh => kh.SoDienThoai == SoDienThoai)
                    .Select(kh => kh.MaKhachHang)
                    .FirstOrDefaultAsync();

                if (maKhachHang == 0)
                {
                    await transaction.RollbackAsync();
                    return Json(new { success = false, message = "Không tìm thấy mã khách hàng vừa tạo." });
                }

                // Đặt hàng cho khách hàng mới
                var maDonHangMoi = new SqlParameter("@MaDonHangMoi", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                await db.Database.ExecuteSqlRawAsync(
                    "EXEC sp_DatHangMoiKhachHang @MaKhachHang, @DiaChi, @GhiChu, @MaDonHangMoi OUTPUT",
                    new SqlParameter("@MaKhachHang", maKhachHang),
                    new SqlParameter("@DiaChi", DiaChi ?? "Chưa cập nhật"),
                    new SqlParameter("@GhiChu", GhiChu ?? string.Empty),
                    maDonHangMoi
                );

                // Commit transaction nếu mọi thứ thành công
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Đặt hàng thành công!", MaDonHang = maDonHangMoi.Value });
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu gặp lỗi
                await transaction.RollbackAsync();
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }






    }
}
