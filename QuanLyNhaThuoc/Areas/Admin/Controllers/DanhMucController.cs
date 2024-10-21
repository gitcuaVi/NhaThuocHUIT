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

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _context.Database.ExecuteSqlRaw("EXEC DeleteDanhMuc @p0", id);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }


    }

}
