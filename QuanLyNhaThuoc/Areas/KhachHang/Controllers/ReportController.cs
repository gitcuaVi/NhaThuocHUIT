using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.Areas.KhachHang.Services;
using QuanLyNhaThuoc.Models;
using System.Data;

namespace QuanLyNhaThuoc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang/[controller]")]
    public class ReportController : Controller
    {
        private readonly QL_NhaThuocContext db;
            public ReportController(QL_NhaThuocContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("ShowOrderDetails/{maDonHang}")]
        public async Task<IActionResult> ShowOrderDetails(int maDonHang)
        {
            // Gọi stored procedure để lấy dữ liệu
            var parameters = new[]
            {
        new SqlParameter("@MaDonHang", SqlDbType.Int) { Value = maDonHang }
    };

            var queryResult = db
                .ThongTinDatHangViewModels
                .FromSqlRaw("EXEC sp_GetThongTinDatHang @MaDonHang", parameters)
                .ToList();

            if (!queryResult.Any())
            {
                return NotFound("Không tìm thấy đơn hàng!");
            }

            // Tạo model để truyền vào View
            var firstResult = queryResult.First();
            var model = new ReportOrder
            {
                code = $"ORD{firstResult.MaDonHang:D8}",
                CreateOn = DateTime.Now,
                DiaChi = new OrderAddess
                {
                    TenKhachHang = firstResult.TenKhachHang,
                    SoDienThoai = firstResult.SoDienThoai,
                    DiaChi = firstResult.DiaChi
                },
                Details = queryResult.Select(item => new OrderDetail
                {
                    TenSanPham = item.TenSanPham,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia,
                   ThanhTien = item.SoLuong*item.DonGia,
                }),
                TongTien = queryResult.Sum(item => item.SoLuong * item.DonGia)
            };

            // Trả về view để hiển thị thông tin đơn hàng
            return View( model);
        }

    }
}
