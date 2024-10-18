using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class NguoiDung
    {
        public NguoiDung()
        {
            KhachHangs = new HashSet<KhachHang>();
            NhanViens = new HashSet<NhanVien>();
        }

        public int MaNguoiDung { get; set; }
        public string TenNguoiDung { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string SoDienThoai { get; set; } = null!;
        public int MaVaiTro { get; set; }
        public string TrangThai { get; set; } = null!;
        public DateTime NgayTao { get; set; }

        public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;
        public virtual ICollection<KhachHang> KhachHangs { get; set; }
        public virtual ICollection<NhanVien> NhanViens { get; set; }
    }
}
