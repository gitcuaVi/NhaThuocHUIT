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
            // lấy danh mục
            var danhMucList = await db.DanhMucs.ToListAsync();
            ViewBag.DanhMucList = danhMucList;

            // lấy giỏ hàng
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            ViewBag.Cart = cart;

            // tổng tiền giỏ hàng
            var total = cart.Sum(x => x.SoLuong * x.DonGia);
            ViewBag.Total = total;

            if (categoryId == null)
            {
                return View(new List<SanPhamViewModel>());
            }

            // lấy danh sách thuốc theo danh mục
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
                // Tìm thuốc theo mã thuốc
                var thuoc = await db.Thuocs.FindAsync(maThuoc);
                if (thuoc == null)
                {
                    //thông báo lỗi
                    TempData["ErrorMessage"] = "Không tìm thấy thuốc.";
                    return RedirectToAction("Index");
                }
                //ktra soluong
                if (thuoc.SoLuongTon < soLuong)
                {
                    TempData["ErrorMessage"] = "Số lượng không đủ.";
                    return RedirectToAction("Index");
                }

                //gio hang tu session
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

                //thuoc ton tai trong gio
                var item = cart.FirstOrDefault(c => c.MaThuoc == maThuoc);
                if (item != null)
                {
                    item.SoLuong += soLuong;
                }
                else
                {
                    //chua them thuoc
                    cart.Add(new CartItem
                    {
                        MaThuoc = maThuoc,
                        TenThuoc = thuoc.TenThuoc,
                        DonGia = thuoc.DonGia,
                        SoLuong = soLuong
                    });
                }
                //up gio hang
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                TempData["SuccessMessage"] = "Thêm vào giỏ hàng thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction("Index");
            }
        }



        //lấy nhân viên từ claim
        private int GetMaNhanVienFromClaims()
        {
            if (User.IsInRole("NhanVien"))
            {
                return int.Parse(User.FindFirstValue("MaNhanVien"));
            }
            return -1;
        }


        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(ThongTinDonHangViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Thông tin không hợp lệ!";
                return RedirectToAction("Index", "NhanVienHoaDon");
            }

            // lấy giỏ hàng
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "NhanVienHoaDon");
            }

            // tạo giỏ hàng
            var chiTietDonHang = new DataTable();
            chiTietDonHang.Columns.Add("MaThuoc", typeof(int));
            chiTietDonHang.Columns.Add("SoLuong", typeof(int));
            chiTietDonHang.Columns.Add("Gia", typeof(decimal));

            // Thêm hàng trong giỏ
            foreach (var item in cart)
            {
                chiTietDonHang.Rows.Add(item.MaThuoc, item.SoLuong, item.DonGia);
            }

            var tongTien = cart.Sum(c => c.SoLuong * c.DonGia);
            var maNhanVien = GetMaNhanVienFromClaims();

            try
            {
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

                // xóa
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


        [HttpGet]
        [Route("Admin/NhanVienHoaDon/DanhSachDonHangNhanVien")]
        public async Task<IActionResult> DanhSachDonHangNhanVien()
        {
            var maNhanVien = GetMaNhanVienFromClaims();

            // Lấy danh sách đơn hàng của nhân viên
            var donHangList = await db.DonHangs
                .Where(d => d.MaNhanVien == maNhanVien)
                .OrderByDescending(d => d.NgayDatHang)//giam
                .Select(d => new DonHangViewModel
                {
                    MaDonHang = d.MaDonHang,
                    TongTien = d.TongTien,
                    NgayDatHang = d.NgayDatHang,
                    TrangThai = d.TrangThai,
                    ChiTietDonHang = d.ChiTietDonHangs.Select(ct => new ChiTietDonHangNhanVienViewModel
                    {
                        TenThuoc = ct.Thuoc.TenThuoc,
                        SoLuong = ct.SoLuong,
                        Gia = ct.Gia
                    }).ToList()
                })
                .ToListAsync();

            return View(donHangList);
        }


    }
}
