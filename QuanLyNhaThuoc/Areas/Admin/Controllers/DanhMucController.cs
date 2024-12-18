using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
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
        public IActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            // Lấy danh mục
            var danhMucQuery = _context.DanhMucs.AsQueryable();

            //Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                danhMucQuery = danhMucQuery.Where(dm => dm.TenDanhMuc.Contains(searchString));
            }

            int totalItems = danhMucQuery.Count();

            // Lấy danh mục, sắp xếp danh mục
            var danhMucList = danhMucQuery
                              .OrderBy(dm => dm.MaDanhMuc) 
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToList();

            // gửi đến View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentFilter = searchString;

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
            if (ModelState.IsValid) //hop le
            {
                try
                {
                    var parameters = new object[] { danhMuc.TenDanhMuc, danhMuc.LoaiMenu };
                    _context.Database.ExecuteSqlRaw("EXEC CreateDanhMuc @p0, @p1", parameters);

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
            if (id != danhMuc.MaDanhMuc) // khong hop le madanhmuc
            {
                return NotFound();
            }

            if (ModelState.IsValid) // hop le
            {
                try
                {
                    var parameters = new object[] { danhMuc.MaDanhMuc, danhMuc.TenDanhMuc, danhMuc.LoaiMenu };
                    _context.Database.ExecuteSqlInterpolated($"EXEC UpdateDanhMuc {danhMuc.MaDanhMuc}, {danhMuc.TenDanhMuc}, {danhMuc.LoaiMenu}");
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
                // Kiểm tra danh mục
                if (ids == null || ids.Count == 0)
                {
                    return Json(new { success = false, message = "Không có loại sản phẩm nào được chọn để xóa." });
                }
                // Tạo ds id chuỗi
                var idList = string.Join(",", ids);

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
