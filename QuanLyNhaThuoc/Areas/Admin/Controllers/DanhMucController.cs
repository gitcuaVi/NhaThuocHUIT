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

        // GET: /admin/danhmuc
        [HttpGet("")]
        public IActionResult Index()
        {
            var danhMucList = _context.DanhMucs
                              .Include(dm => dm.LoaiSanPhams)
                              .ToList();
            return View(danhMucList);
        }

        // GET: /admin/danhmuc/create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /admin/danhmuc/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DanhMuc danhMuc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(danhMuc);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(danhMuc);
        }
    }

}
