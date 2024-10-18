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

        [HttpGet]
        public IActionResult Index()
        {
            var danhMucList = _context.DanhMucs
                              .Include(dm => dm.LoaiSanPhams) 
                              .ToList();
            return View(danhMucList);
        }
    }
}
