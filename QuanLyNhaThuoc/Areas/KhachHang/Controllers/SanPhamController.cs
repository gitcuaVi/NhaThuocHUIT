using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuanLyNhaThuoc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using System.Security.Claims;

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
            var cartItems = await db.Set<GioHangViewModel>()
                .FromSqlRaw("EXEC sp_GetGioHangByKhachHang @MaKhachHang", paramMaKhachHang)
                .ToListAsync();

            return View(cartItems);
        }



    }
}

