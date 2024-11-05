using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class GioHang
    {
        public int MaGioHang { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal TongTien { get; set; }
        public int MaThuoc { get; set; }

        public virtual Thuoc MaThuocNavigation { get; set; } = null!;
    }
}
