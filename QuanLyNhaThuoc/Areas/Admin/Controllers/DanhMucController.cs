using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class DanhMucController : Controller
    {
        private readonly QL_NhaThuocContext _context;

        public DanhMucController(QL_NhaThuocContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            var danhMucList = _context.DanhMucs
                              .FromSqlRaw("EXEC GetDanhMucList")
                              .ToList();
            return View(danhMucList);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DanhMuc danhMuc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var parameters = new[] { danhMuc.TenDanhMuc };
                    _context.Database.ExecuteSqlRaw("EXEC CreateDanhMuc @p0", parameters);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo danh mục: " + ex.Message);
                }
            }
            return View(danhMuc);
        }

        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var danhMuc = _context.DanhMucs.FirstOrDefault(dm => dm.MaDanhMuc == id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            return View(danhMuc);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DanhMuc danhMuc)
        {
            if (id != danhMuc.MaDanhMuc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var parameters = new object[] { danhMuc.MaDanhMuc, danhMuc.TenDanhMuc };
                    _context.Database.ExecuteSqlInterpolated($"EXEC UpdateDanhMuc {danhMuc.MaDanhMuc}, {danhMuc.TenDanhMuc}");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DanhMucs.Any(dm => dm.MaDanhMuc == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(danhMuc);
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return Json(new { success = false, message = "Không có loại sản phẩm nào được chọn để xóa." });
                }

                // Tạo danh sách ID dưới dạng chuỗi để sử dụng trong câu truy vấn
                var idList = string.Join(",", ids);

                // Gọi stored procedure xóa nhiều sản phẩm cùng lúc
                _context.Database.ExecuteSqlRaw($"EXEC sp_XoaDanhMuc @Ids = '{idList}'");

                return Json(new { success = true, message = "Các loại sản phẩm đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }




    }

}
