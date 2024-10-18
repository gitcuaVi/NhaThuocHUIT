using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class ChiTietDonHang
    {
        public int MaChiTiet { get; set; }
        public int MaDonHang { get; set; }
        public int SoLuong { get; set; }
        public int MaThuoc { get; set; }
        public decimal Gia { get; set; }

        public virtual DonHang MaDonHangNavigation { get; set; } = null!;
        public virtual Thuoc MaThuocNavigation { get; set; } = null!;
    }
}
