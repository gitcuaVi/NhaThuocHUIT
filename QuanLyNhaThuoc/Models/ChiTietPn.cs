using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class ChiTietPn
    {
        public int MaChiTietPn { get; set; }
        public int MaThuoc { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaXuat { get; set; }
        public int MaTonKho { get; set; }
        public int MaPhieuNhap { get; set; }

        public bool? TrangThai { get; set; }


        public virtual PhieuNhap MaPhieuNhapNavigation { get; set; } = null!;
        public virtual Thuoc MaThuocNavigation { get; set; } = null!;
        public virtual TonKho MaTonKhoNavigation { get; set; } = null!;
    }
}
