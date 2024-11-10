using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaThuoc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang/[controller]")]
    public class GocSucKhoeController : Controller
    {
        // Action for the Index page
      

        // Action for Blog Sức Khỏe
        [HttpGet("Blogsuckhoe")]
        public IActionResult Blogsuckhoe()
        {
            return View();
        }

        // Action for Tra Cứu Bệnh
        [HttpGet("Tracuubenh")]
        public IActionResult Tracuubenh()
        {
            return View();
        }
    }
}
