using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class ThanhToan
    {
        public int MaThanhToan { get; set; }
        public int MaDonHang { get; set; }
        public string PhuongThucThanhToan { get; set; } = null!;
        public string TrangThaiThanhToan { get; set; } = null!;
        public DateTime? NgayThanhToan { get; set; }
        public decimal SoTien { get; set; }
        public string? MaQr { get; set; }
        public string? GhiChu { get; set; }

        public virtual DonHang MaDonHangNavigation { get; set; } = null!;
    }
}
