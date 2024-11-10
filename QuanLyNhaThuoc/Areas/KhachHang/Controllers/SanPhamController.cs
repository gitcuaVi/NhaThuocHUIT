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

        [HttpGet("Partial_TuThuocGiaDinh")]
        public IActionResult Partial_TuThuocGiaDinh()
        {
            return PartialView();
        }

        [HttpGet("GetProductsByDanhMuc")]
        public IActionResult GetProductsByDanhMuc(int maDanhMuc = 17)
        {
            var products = db.Thuocs
                .Include(t => t.HinhAnhs)
                .Where(t => t.MaLoaiSanPhamNavigation.MaDanhMuc == maDanhMuc)
                .Select(t => new ProductViewModel
                {
                    TenThuoc = t.TenThuoc,
                    DonGia = t.DonGia,
                    UrlAnh = t.HinhAnhs.FirstOrDefault().UrlAnh,
                    DonVi = t.DonVi
                }).ToList();

            return PartialView("_ProductList", products);
        }
    }
}
