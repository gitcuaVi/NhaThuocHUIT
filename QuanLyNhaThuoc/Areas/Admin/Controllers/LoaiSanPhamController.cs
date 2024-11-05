using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using QuanLyNhaThuoc.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "NhanVien")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class LoaiSanPhamController : Controller
    {
        private readonly QL_NhaThuocContext _context;

        public LoaiSanPhamController(QL_NhaThuocContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách loại sản phẩm
        [HttpGet("")]
        public IActionResult Index()
        {
            var loaiSanPhamList = _context.LoaiSanPhams
                                  .Include(lsp => lsp.MaDanhMucNavigation)
                                  .ToList();
            return View(loaiSanPhamList);
        }

        // GET: Trang thêm loại sản phẩm
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc");
            return View();
        }

        // POST: Thêm loại sản phẩm mới
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string TenLoai, int MaDanhMuc)
        {
            try
            {
                // Tạo danh sách tham số cho stored procedure
                var parameters = new[]
                {
                    new SqlParameter("@TenLoai", TenLoai),
                    new SqlParameter("@MaDanhMuc", MaDanhMuc)
                };

                // Gọi stored procedure để thêm loại sản phẩm
                var result = _context.Database.ExecuteSqlRaw("EXEC sp_ThemLoaiSanPham @TenLoai, @MaDanhMuc", parameters);

                if (result == 0)
                {
                    ModelState.AddModelError("", "Lỗi: Loại sản phẩm đã tồn tại trong danh mục này");
                    return View();
                }

                // Thành công
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View();
            }
        }
        // GET: Trang chỉnh sửa loại sản phẩm
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var loaiSanPham = _context.LoaiSanPhams
                                    .FirstOrDefault(lsp => lsp.MaLoaiSanPham == id);

            if (loaiSanPham == null)
            {
                return NotFound();
            }

            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", loaiSanPham.MaDanhMuc);
            return View(loaiSanPham);
        }

        // POST: Chỉnh sửa loại sản phẩm
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string TenLoai, int MaDanhMuc)
        {
            try
            {
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
                foreach (var id in MaLoaiSanPham)
                {
                    var parameters = new[]
                    {
                new SqlParameter("@MaLoaiSanPham", id)
            };

                    // Call the stored procedure to delete the product type
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
