using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class GioHang
    {
        public int MaGioHang { get; set; }
        public decimal TongTien { get; set; }
        public int? MaKhachHang { get; set; }
        public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;
        public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();
    }
}
