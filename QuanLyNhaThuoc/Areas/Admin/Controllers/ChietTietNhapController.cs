using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class ChietTietNhapController : Controller
    {
        private readonly QL_NhaThuocContext _context;
        public ChietTietNhapController(QL_NhaThuocContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
