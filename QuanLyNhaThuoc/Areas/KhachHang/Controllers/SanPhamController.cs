using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuanLyNhaThuoc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace QuanLyNhaThuoc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang/[controller]")]
    public class SanPhamController : Controller
    {
        private readonly QL_NhaThuocContext db;

        public SanPhamController(QL_NhaThuocContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int categoryId, decimal? minPrice, decimal? maxPrice, string productType)
        {
            var paramCategoryId = new SqlParameter("@MaDanhMuc", categoryId);
            var paramMinPrice = new SqlParameter("@MinPrice", minPrice ?? (object)DBNull.Value);
            var paramMaxPrice = new SqlParameter("@MaxPrice", maxPrice ?? (object)DBNull.Value);
            var paramProductType = new SqlParameter("@ProductType", string.IsNullOrEmpty(productType) ? (object)DBNull.Value : productType);

            var products = await db.Set<ProductViewModel>()
                .FromSqlRaw("EXEC sp_GetAllProductsByCategoryAndFilters @MaDanhMuc, @MinPrice, @MaxPrice, @ProductType",
                            paramCategoryId, paramMinPrice, paramMaxPrice, paramProductType)
                .ToListAsync();

            var productTypes = await db.LoaiSanPhams
                .Where(l => l.MaDanhMuc == categoryId)
                .ToListAsync();

            ViewData["ProductTypes"] = productTypes;

            ViewData["Products"] = products;

            return View();
        }

        private int GetMaKhachHangFromClaims()
        {
            var maKhachHangClaim = User.FindFirst("MaKhachHang")?.Value;
            return int.TryParse(maKhachHangClaim, out var maKhachHang) ? maKhachHang : -1;
        }

        [HttpGet("Chitietsp/{id}")]
        public async Task<IActionResult> Chitietsp(int id)
        {
            var paramId = new SqlParameter("@MaThuoc", id);
            var productDetail = await db.Set<ProductViewDetailsModel>()
                .FromSqlRaw("SELECT * FROM vw_ChiTietThuoc WHERE MaThuoc = @MaThuoc", paramId)
                .FirstOrDefaultAsync();

            if (productDetail == null)
            {
                return NotFound();
            }

            return View(productDetail);

        }


        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(int maThuoc, int soLuong)
        {
            try
            {
                int maKhachHang = GetMaKhachHangFromClaims();

                await db.Database.ExecuteSqlRawAsync("EXEC sp_AddToCart @MaKhachHang, @MaThuoc, @SoLuong",
                    new SqlParameter("@MaKhachHang", maKhachHang),
                    new SqlParameter("@MaThuoc", maThuoc),
                    new SqlParameter("@SoLuong", soLuong)
                );

                return Json(new { success = true, message = "Thêm vào giỏ hàng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet("Cart")]
        public async Task<IActionResult> Cart()
        {
            int maKhachHang = GetMaKhachHangFromClaims();
            if (maKhachHang == -1)
            {
                return RedirectToAction("Login", "UserDH");
            }

            var paramMaKhachHang = new SqlParameter("@MaKhachHang", maKhachHang);

            // Lấy thông tin khách hàng
            var customerInfo = db.Set<ThongTinKhachHangGioHang>()
                .FromSqlRaw("EXEC sp_GetThongTinKhachHangGioHang @MaKhachHang", paramMaKhachHang)
                .AsEnumerable() 
                .FirstOrDefault(); 

            if (customerInfo != null)
            {
                ViewBag.HoTen = customerInfo.TenKhachHang;
                ViewBag.SoDienThoai = customerInfo.SoDienThoai;
                ViewBag.DiaChi = customerInfo.DiaChi;
            }

            // Lấy số lượng sản phẩm trong giỏ
            var cartCount = await db.Database
                .ExecuteSqlRawAsync("EXEC sp_GetCartCountByKhachHang @MaKhachHang", paramMaKhachHang);

            ViewData["CartCount"] = cartCount;

            // Lấy chi tiết giỏ hàng
            var cartItems = await db.Set<GioHangViewModel>()
                .FromSqlRaw("EXEC sp_GetGioHangByKhachHang @MaKhachHang", paramMaKhachHang)
                .ToListAsync();

            return View(cartItems);
        }




        [HttpGet("GetCartCount")]
        public async Task<IActionResult> GetCartCount()
        {
            int maKhachHang = GetMaKhachHangFromClaims();
            if (maKhachHang == -1)
            {
                return Json(new { count = 0 });
            }

            // Lấy giỏ hàng của khách hàng hiện tại
            var gioHang = await db.GioHangs
                .FirstOrDefaultAsync(g => g.MaKhachHang == maKhachHang);

            if (gioHang == null)
            {
                return Json(new { count = 0 });
            }

            var cartCount = await db.ChiTietGioHangs
                .Where(c => c.MaGioHang == gioHang.MaGioHang)
                .SumAsync(c => c.SoLuong);

            return Json(new { count = cartCount });
        }

        [HttpPost("UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityModel model)
        {
            int maKhachHang = GetMaKhachHangFromClaims();
            if (model == null || model.MaChiTietGioHang <= 0 || model.SoLuong <= 0)
            {
                Console.WriteLine("Invalid data: " + model);
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }


            try
            {
                var paramMaChiTietGioHang = new SqlParameter("@MaChiTietGioHang", model.MaChiTietGioHang);
                var paramSoLuong = new SqlParameter("@SoLuong", model.SoLuong);

                await db.Database.ExecuteSqlRawAsync("EXEC sp_UpdateCartItemQuantity @MaChiTietGioHang, @SoLuong", paramMaChiTietGioHang, paramSoLuong);

                return Json(new { success = true, message = "Cập nhật số lượng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart([FromBody] int maChiTietGioHang)
        {
            int maKhachHang = GetMaKhachHangFromClaims();
            try
            {
                var paramMaChiTietGioHang = new SqlParameter("@MaChiTietGioHang", maChiTietGioHang);

                int rowsAffected = await db.Database.ExecuteSqlRawAsync("EXEC sp_RemoveFromCart @MaChiTietGioHang", paramMaChiTietGioHang);

                if (rowsAffected > 0)
                    return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
                else
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm để xóa." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }




    }
}


