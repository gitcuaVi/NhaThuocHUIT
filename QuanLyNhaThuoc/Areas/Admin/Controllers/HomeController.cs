using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
   [Area("Admin")]
   
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [Route("Home/AccessDenied")]
        public IActionResult AccessDenied(string returnUrl)
        {
            // Check if ReturnUrl is an AccessDenied page, avoid infinite redirect
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.Contains("AccessDenied"))
            {
                return RedirectToAction("Index", "Home"); // Redirect to a default page
            }
            return View();
        }


    }
}