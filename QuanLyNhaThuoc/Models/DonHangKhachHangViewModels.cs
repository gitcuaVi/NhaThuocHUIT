using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaThuoc.Models
{
    public class DonHangKhachHangViewModels
    {
        public int MaDonHang { get; set; }
        public decimal TongTien { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayDatHang { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public string? DiaChiDonHang { get; set; }  // Tương ứng với DiaChi của đơn hàng
        public DateTime? NgayGiaoHang { get; set; } = null!;
        public string? TrangThai { get; set; }
        public int MaKhachHang { get; set; }
        public string? TenKhachHang { get; set; }
        public string? GioiTinh { get; set; }
        public string? DiaChiKhachHang { get; set; }  // Tương ứng với DiaChi của khách hàng
        public DateTime NgaySinh { get; set; }
        public string? SoDienThoai { get; set; }
        public int MaNhanVien { get; set; }

    }

}
