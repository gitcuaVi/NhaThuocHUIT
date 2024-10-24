using Microsoft.AspNetCore.Mvc;
//using QuanLyNhaThuoc.Areas.Admin.Data;
using QuanLyNhaThuoc.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }


    }
}