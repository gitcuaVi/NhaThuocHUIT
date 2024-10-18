using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class SaoLuuVaPhucHoi
    {
        public int MaSaoLuu { get; set; }
        public int? MaNhanVien { get; set; }
        public DateTime? ThoiGianSaoLuu { get; set; }
        public DateTime? ThoiGianPhucHoi { get; set; }
        public string? TrangThaiSaoLuu { get; set; }
        public string? DiaChi { get; set; }
        public string? TenFileSaoLuu { get; set; }

        public virtual NhanVien? MaNhanVienNavigation { get; set; }
    }
}
