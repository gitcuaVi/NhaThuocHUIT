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
using ViewModelChiTiet = QuanLyNhaThuoc.ViewModels.ChiTietDonHangNhanVienViewModel;
using ModelChiTiet = QuanLyNhaThuoc.Models.ChiTietDonHangViewModel;



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

            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            ViewBag.Cart = cart;

            // Tính tổng tiền giỏ hàng
            var total = cart.Sum(x => x.SoLuong * x.DonGia);
            ViewBag.Total = total;

            // Nếu không có categoryId, hiển thị trang mặc định
            if (categoryId == null)
            {
                return View(new List<SanPhamViewModel>());
            }

            // Lấy danh sách thuốc theo danh mục
            var thuocList = await db.Thuocs
                .Where(t => t.MaLoaiSanPhamNavigation != null && t.MaLoaiSanPhamNavigation.MaDanhMuc == categoryId)
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



        [HttpPost]
        [Route("Admin/NhanVienHoaDon/AddToCart")]
        public async Task<IActionResult> AddToCart(int maThuoc, int soLuong)
        {
            try
            {
                // Lấy thông tin thuốc
                var thuoc = await db.Thuocs.FindAsync(maThuoc);
                if (thuoc == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thuốc." });
                }

                if (thuoc.SoLuongTon < soLuong)
                {
                    return Json(new { success = false, message = "Số lượng không đủ." });
                }

                // Lưu thông tin vào session giỏ hàng (hoặc lưu vào bảng Giỏ hàng tạm)
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

                var item = cart.FirstOrDefault(c => c.MaThuoc == maThuoc);
                if (item != null)
                {
                    item.SoLuong += soLuong;
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        MaThuoc = maThuoc,
                        TenThuoc = thuoc.TenThuoc,
                        DonGia = thuoc.DonGia,
                        SoLuong = soLuong
                    });
                }

                HttpContext.Session.SetObjectAsJson("Cart", cart);

                return Json(new { success = true, message = "Thêm vào giỏ hàng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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


        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(ThongTinDonHangViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Thông tin không hợp lệ!";
                return RedirectToAction("Index", "NhanVienHoaDon");
            }

            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "NhanVienHoaDon");
            }

            // Tạo DataTable từ giỏ hàng
            var chiTietDonHang = new DataTable();
            chiTietDonHang.Columns.Add("MaThuoc", typeof(int));
            chiTietDonHang.Columns.Add("SoLuong", typeof(int));
            chiTietDonHang.Columns.Add("Gia", typeof(decimal));

            foreach (var item in cart)
            {
                chiTietDonHang.Rows.Add(item.MaThuoc, item.SoLuong, item.DonGia);
            }

            var tongTien = cart.Sum(c => c.SoLuong * c.DonGia);
            var maNhanVien = GetMaNhanVienFromClaims();

            try
            {
                // Gọi stored procedure
                await db.Database.ExecuteSqlRawAsync(
                    "EXEC sp_TaoDonHangVaKhachHang @TenKhachHang, @SoDienThoai, @GioiTinh, @DiaChi, @TongTien, @ChiTietDonHang, @MaNhanVien",
                    new SqlParameter("@TenKhachHang", model.TenKhachHang),
                    new SqlParameter("@SoDienThoai", model.SoDienThoai),
                    new SqlParameter("@GioiTinh", model.GioiTinh),
                    new SqlParameter("@DiaChi", model.DiaChi ?? "Không xác định"),
                    new SqlParameter("@TongTien", tongTien),
                    new SqlParameter("@ChiTietDonHang", chiTietDonHang)
                    {
                        SqlDbType = SqlDbType.Structured,
                        TypeName = "TVP_ChiTietDonHang"
                    },
                    new SqlParameter("@MaNhanVien", maNhanVien == -1 ? (object)DBNull.Value : maNhanVien)
                );

                // Xóa giỏ hàng
                HttpContext.Session.Remove("Cart");

                TempData["Success"] = "Đặt hàng thành công!";
                return RedirectToAction("DanhSachDonHangNhanVien", "NhanVienHoaDon", new { area = "Admin" });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Index", "NhanVienHoaDon");
            }
        }




        [HttpGet("DanhSachDonHangNhanVien")]
        public async Task<IActionResult> DanhSachDonHangNhanVien()
        {
            try
            {
                // Lấy mã nhân viên hiện tại từ Claims
                var maNhanVien = GetMaNhanVienFromClaims();
                if (maNhanVien == -1)
                {
                    return Forbid("Không có quyền truy cập.");
                }

                // Lấy mã đơn hàng gần nhất của nhân viên hiện tại
                var maDonHang = await db.DonHangs
                    .Where(dh => dh.MaNhanVien == maNhanVien)
                    .Select(dh => dh.MaDonHang)
                    .FirstOrDefaultAsync();

                if (maDonHang == 0)
                {
                    return NotFound("Không tìm thấy đơn hàng nào.");
                }

                // Gọi stored procedure để lấy chi tiết đơn hàng
                var orderDetails = await db.ThongTinDatHangViewModels.FromSqlRaw(
                    "EXEC sp_GetThongTinDatHang @MaDonHang",
                    new SqlParameter("@MaDonHang", maDonHang)
                ).ToListAsync();

                if (!orderDetails.Any())
                {
                    return NotFound("Không tìm thấy thông tin chi tiết đơn hàng.");
                }

                // Tạo PaymentModel
                var paymentModel = new PaymentModel
                {
                    MaDonHang = maDonHang,
                    TotalAmount = orderDetails.Sum(x => x.ThanhTien),
                    PaymentMethod = null // Người dùng sẽ chọn
                };

                // Tạo ViewModel tổng hợp
                var viewModel = new OrderDetailsViewModel
                {
                    OrderItems = orderDetails,
                    PaymentInfo = paymentModel
                };

                return View(viewModel); // Truyền ViewModel tổng hợp
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi: " + ex.Message);
            }
        }


    }
}
