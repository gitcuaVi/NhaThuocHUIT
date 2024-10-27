using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;
using System.Data;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class PhieuNhapController : Controller
    {
        private readonly QL_NhaThuocContext _context;

        public PhieuNhapController(QL_NhaThuocContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var phieuNhaps = _context.PhieuNhaps
                .Include(p => p.MaNhanVienNavigation)
                .Include(p => p.ChiTietPns)
                    .ThenInclude(ctpn => ctpn.MaTonKhoNavigation)
                .ToList();

            return View(phieuNhaps);
        }

    }
}
