using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class PhieuXuat
    {
        public PhieuXuat()
        {
            ChiTietPxes = new HashSet<ChiTietPx>();
        }

        public int MaPhieuXuat { get; set; }
        public DateTime? NgayXuat { get; set; }
        public int MaNhanVien { get; set; }
        public decimal TongTien { get; set; }
        public string? GhiChu { get; set; }
        public string? NoiNhan { get; set; }

        public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;
        public virtual ICollection<ChiTietPx> ChiTietPxes { get; set; }
    }
}
