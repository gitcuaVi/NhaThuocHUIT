using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuanLyNhaThuoc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Models;

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


    }
}
