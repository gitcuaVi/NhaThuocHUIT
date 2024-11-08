using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.Models;
using System.Linq;

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
        public IActionResult Partial_TuThuocGiaDinh()
        {
            return PartialView();
        }

        [HttpGet("DanhMuc/{maDanhMuc:int}")]
        public IActionResult Index(int maDanhMuc)
        {
            var products = db.Thuocs
                .Include(t => t.MaLoaiSanPhamNavigation)
                .Where(t => t.MaLoaiSanPhamNavigation.MaDanhMuc == maDanhMuc)
                .Select(t => new
                {
                    t.MaThuoc,
                    t.TenThuoc,
                    t.DonGia,
                    t.SoLuongTon,
                    t.DonVi,
                    HinhAnhUrl = t.HinhAnhs.Select(h => h.UrlAnh).FirstOrDefault()
                })
                .ToList();

            ViewData["SelectDanhMuc"] = db.DanhMucs
                .Where(d => d.MaDanhMuc == maDanhMuc)
                .Select(d => d.TenDanhMuc)
                .FirstOrDefault();

            return View(products);
        }
    }
}
