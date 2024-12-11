using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class PhieuNhap
    {
        public PhieuNhap()
        {
            ChiTietPns = new HashSet<ChiTietPn>();
        }

        public int MaPhieuNhap { get; set; }
        public int MaNhanVien { get; set; }
        public decimal TongTien { get; set; }
        public DateTime NgayNhap { get; set; }
        public string? GhiChu { get; set; }
        public string NhaCungCap { get; set; } = null!;

        public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;
        public virtual ICollection<ChiTietPn> ChiTietPns { get; set; }
    }
}
