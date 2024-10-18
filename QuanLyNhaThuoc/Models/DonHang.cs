using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class DonHang
    {
        public DonHang()
        {
            ChiTietDonHangs = new HashSet<ChiTietDonHang>();
            ThanhToans = new HashSet<ThanhToan>();
        }

        public int MaDonHang { get; set; }
        public decimal TongTien { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayDatHang { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public string DiaChi { get; set; } = null!;
        public DateTime NgayGiaoHang { get; set; }
        public string TrangThai { get; set; } = null!;
        public int MaNhanVien { get; set; }
        public int MaKhachHang { get; set; }

        public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;
        public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public virtual ICollection<ThanhToan> ThanhToans { get; set; }
    }
}
