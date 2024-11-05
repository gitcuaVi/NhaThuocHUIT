using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class Luong
    {
        public int MaLuong { get; set; }
        public int MaNhanVien { get; set; }
        public decimal KhauTru { get; set; }
        public decimal LuongThucNhan { get; set; }
        public DateTime NgayTraLuong { get; set; }
        public string GhiChu { get; set; } = null!;
        public int SoCaLamViec { get; set; }
        public int SoGioTangCa { get; set; }
        public DateTime LuongThang { get; set; }
        public decimal? LuongThuong { get; set; }

        public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;
    }
}
