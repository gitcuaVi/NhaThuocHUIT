using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.Models;
using QuanLyNhaThuoc.ViewModels;
using System.Data;
using System.Security.Claims;
using KhachHangModel = QuanLyNhaThuoc.Models.KhachHang;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class NhanVienHoaDonController : Controller
    {

        private readonly QL_NhaThuocContext db;

        public NhanVienHoaDonController(QL_NhaThuocContext context)
        {
            db = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId)
        {
            // Lấy danh sách danh mục
            var danhMucList = await db.DanhMucs.ToListAsync();
            ViewBag.DanhMucList = danhMucList;

            // Nếu không có categoryId, hiển thị trang mặc định
            if (categoryId == null)
            {
                return View(new List<SanPhamViewModel>());
            }

            // Lấy danh sách thuốc theo danh mục
            var thuocList = await db.Thuocs
                .Where(t => t.MaLoaiSanPhamNavigation.MaDanhMuc == categoryId)
                .Include(t => t.HinhAnhs)
                .Select(t => new SanPhamViewModel
                {
                    MaThuoc = t.MaThuoc,
                    TenThuoc = t.TenThuoc,
                    HinhAnhDauTien = t.HinhAnhs.FirstOrDefault().UrlAnh,
                    DonGia = t.DonGia,
                    DonVi = t.DonVi
                })
                .ToListAsync();

            return View(thuocList);
        }





    }
}
