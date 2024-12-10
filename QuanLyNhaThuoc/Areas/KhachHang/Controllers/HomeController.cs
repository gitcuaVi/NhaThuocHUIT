using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyNhaThuoc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang/[controller]")]
    public class HomeController : Controller
    {
        private readonly QL_NhaThuocContext db;

        public HomeController(QL_NhaThuocContext context)
        {
            db = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int maDanhMuc1 = 97;
            int maDanhMuc2 = 98;

            var param1 = new SqlParameter("@MaDanhMuc", maDanhMuc1);
            var param2 = new SqlParameter("@MaDanhMuc", maDanhMuc2);

            var products1 = await db.Set<ProductViewModel>()
                .FromSqlRaw("EXEC sp_GetAllProductsByCategory @MaDanhMuc", param1)
                .ToListAsync();

            var products2 = await db.Set<ProductViewModel>()
                .FromSqlRaw("EXEC sp_GetAllProductsByCategory @MaDanhMuc", param2)
                .ToListAsync();

            ViewData["Products1"] = products1;
            ViewData["Products2"] = products2;

            return View();
        }
        [HttpGet("Gioithieu")]
        public async Task<IActionResult> Gioithieu()
        {
            return View();
        }
        [HttpGet("BanDieuHanh")]
        public async Task<IActionResult> BanDieuHanh()
        {
            return View();
        }
        [HttpGet("ChinhSach")]
        public async Task<IActionResult> ChinhSach()
        {
            return View();
        }
    }
}