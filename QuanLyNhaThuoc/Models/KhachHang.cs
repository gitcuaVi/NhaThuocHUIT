using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class KhachHang
    {
        public KhachHang()
        {
            DonHangs = new HashSet<DonHang>();
        }

        public int MaKhachHang { get; set; }
        public string TenKhachHang { get; set; } = null!;
        public string GioiTinh { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public DateTime NgaySinh { get; set; }
        public string SoDienThoai { get; set; } = null!;
        public int? MaNguoiDung { get; set; }
        public int Diem { get; set; }

        public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
        public virtual ICollection<DonHang> DonHangs { get; set; }
    }
}
