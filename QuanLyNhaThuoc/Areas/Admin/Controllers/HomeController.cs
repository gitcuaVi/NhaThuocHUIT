using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.Models;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly QL_NhaThuocContext db;

        public HomeController(QL_NhaThuocContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
          
        }

        public IActionResult GetDonHang(int year = 2024)
        {
            var monthlyOrders = db.DonHangs
                .Where(d => d.NgayDatHang.Year == year) 
                .GroupBy(d => d.NgayDatHang.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

            var allMonths = Enumerable.Range(1, 12).Select(i => new
            {
                Month = i,
                OrderCount = monthlyOrders.FirstOrDefault(x => x.Month == i)?.OrderCount ?? 0
            }).ToList();

            return Json(allMonths);
        }
        public IActionResult GetPhieuNhap(int year = 2024)
        {
            var monthlyInvoices = db.PhieuNhaps
                .Where(p => p.NgayNhap.Year == year)
                .GroupBy(p => p.NgayNhap.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    InvoiceCount = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

            var allMonths = Enumerable.Range(1, 12).Select(i => new
            {
                Month = i,
                InvoiceCount = monthlyInvoices.FirstOrDefault(x => x.Month == i)?.InvoiceCount ?? 0
            }).ToList();

            return Json(allMonths);
        }


        [Route("Home/AccessDenied")]
        public IActionResult AccessDenied(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.Contains("AccessDenied"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        

    }
}
