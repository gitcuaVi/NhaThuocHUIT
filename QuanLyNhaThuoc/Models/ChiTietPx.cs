using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class ChiTietPx
    {
        public int MaChiTietPx { get; set; }
        public int MaPhieuXuat { get; set; }
        public decimal DonGiaXuat { get; set; }
        public int SoLuong { get; set; }
        public int MaTonKho { get; set; }
        public string DonVi { get; set; } = null!;
        public int MaThuoc { get; set; }

        public virtual PhieuXuat MaPhieuXuatNavigation { get; set; } = null!;
        public virtual Thuoc MaThuocNavigation { get; set; } = null!;
        public virtual TonKho MaTonKhoNavigation { get; set; } = null!;
    }
}
