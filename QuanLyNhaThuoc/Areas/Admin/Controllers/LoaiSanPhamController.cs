using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using QuanLyNhaThuoc.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
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
        public IActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var loaiSanPhamQuery = _context.LoaiSanPhams
                .Include(lsp => lsp.MaDanhMucNavigation)
                .AsQueryable();

            // tìm kiếm thì lọc
            if (!string.IsNullOrEmpty(searchString))
            {
                loaiSanPhamQuery = loaiSanPhamQuery.Where(lsp =>
                    lsp.TenLoai.Contains(searchString) ||
                    lsp.MaDanhMucNavigation.TenDanhMuc.Contains(searchString));
            }

            int totalItems = loaiSanPhamQuery.Count();

            //phân trang
            var loaiSanPhamList = loaiSanPhamQuery
                .OrderBy(lsp => lsp.MaLoaiSanPham)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // phân trang và tìm kiếm đến View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentFilter = searchString;

            return View(loaiSanPhamList);
        }


        [HttpGet("Create")]
        public IActionResult Create()
        {
            // Lấy danh sách danh mục
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc");
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string TenLoai, int MaDanhMuc)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@TenLoai", TenLoai),
                    new SqlParameter("@MaDanhMuc", MaDanhMuc)
                };

                var result = _context.Database.ExecuteSqlRaw("EXEC sp_ThemLoaiSanPham @TenLoai, @MaDanhMuc", parameters);

                if (result == 0)
                {
                    ModelState.AddModelError("", "Lỗi: Loại sản phẩm đã tồn tại trong danh mục này");
                    return View();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View();
            }
        }


        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            //lay loaisanpham 
            var loaiSanPham = _context.LoaiSanPhams
                                    .FirstOrDefault(lsp => lsp.MaLoaiSanPham == id);

            if (loaiSanPham == null)
            {
                return NotFound();
            }

            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", loaiSanPham.MaDanhMuc);
            return View(loaiSanPham);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string TenLoai, int MaDanhMuc)
        {
            try
            {
                // Tạo tham số loại sản phẩm
                var parameters = new[]
                {
            new SqlParameter("@MaLoaiSanPham", id),
            new SqlParameter("@TenLoai", TenLoai),
            new SqlParameter("@MaDanhMuc", MaDanhMuc)
        };

                _context.Database.ExecuteSqlRaw("EXEC sp_SuaLoaiSanPham @MaLoaiSanPham, @TenLoai, @MaDanhMuc", parameters);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View();
            }
        }

        [HttpPost]
        [Route("/Admin/LoaiSanPham/Delete")]
        public IActionResult Delete(int[] MaLoaiSanPham)
        {
            try
            {
                // Duyệt qua danh sách loại sản phẩm xóa
                foreach (var id in MaLoaiSanPham)
                {
                    var parameters = new[]
                    {
                new SqlParameter("@MaLoaiSanPham", id)
            };

                    _context.Database.ExecuteSqlRaw("EXEC sp_XoaLoaiSanPham @MaLoaiSanPham", parameters);
                }

                return Json(new { success = true, message = "Loại sản phẩm đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

    }
}
