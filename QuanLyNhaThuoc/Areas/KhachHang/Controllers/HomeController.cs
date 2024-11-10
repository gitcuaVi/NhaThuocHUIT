using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyNhaThuoc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang/[controller]")]
    public class HomeController : Controller
    {
        private readonly QL_NhaThuocContext db;

        public HomeController(QL_NhaThuocContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpGet("Gioithieu")]
        public async Task<IActionResult> Gioithieu()
        {
            return View();
        }
        [HttpGet("BanDieuHanh")]
        public async Task<IActionResult> BanDieuHanh()
        {
            return View();
        }
        [HttpGet("ChinhSach")]
        public async Task<IActionResult> ChinhSach()
        {
            return View();
        }
    }
}
