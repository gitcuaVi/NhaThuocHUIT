using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [Route("Account/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}