using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class LoaiSanPhamController : Controller
    {
        private readonly QL_NhaThuocContext _context;

        public LoaiSanPhamController(QL_NhaThuocContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var loaiSanPhamList = _context.LoaiSanPhams
                                          .Include(lsp => lsp.MaDanhMucNavigation)
                                          .ToList();
            return View(loaiSanPhamList);
        }


        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.MaDanhMuc = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc");
            return View();
        }


        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LoaiSanPham loaisanpham)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var parameters = new[]
                    {
                loaisanpham.TenLoai,
                loaisanpham.MaDanhMuc.ToString() 
            };
                    _context.Database.ExecuteSqlRaw("EXEC sp_ThemLoaiSanPham @p0, @p1", parameters);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo loại sản phẩm: " + ex.Message);
                }
            }

            ViewBag.MaDanhMuc = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc");
            return View(loaisanpham);
        }
    }
}
