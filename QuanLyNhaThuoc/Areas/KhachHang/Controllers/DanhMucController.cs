using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaThuoc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang/[controller]")]
    public class DanhMucController : Controller
    {
        private readonly QL_NhaThuocContext db;

        public DanhMucController(QL_NhaThuocContext context)
        {
            db = context;
        }
        public IActionResult GetDanhMucByLoaiMenu(int loaiMenu)
        {
            var danhMucs = db.DanhMucs
                .Where(d => d.LoaiMenu == loaiMenu)
                .ToList();

            return PartialView("_DanhMucPartial", danhMucs);
        }


    }
}
