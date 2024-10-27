using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class NhanVien
    {
        public NhanVien()
        {
            ChamCongs = new HashSet<ChamCong>();
            DonHangs = new HashSet<DonHang>();
            Luongs = new HashSet<Luong>();
            PhieuNhaps = new HashSet<PhieuNhap>();
            SaoLuuVaPhucHois = new HashSet<SaoLuuVaPhucHoi>();
        }

        public int MaNhanVien { get; set; }
        public string Ho { get; set; } = null!;
        public string Ten { get; set; } = null!;
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public string ChucVu { get; set; } = null!;
        public DateTime NgayTuyenDung { get; set; }
        public string TrangThai { get; set; } = null!;
        public int? MaNguoiDung { get; set; }
        public int MaCaLamViec { get; set; }
        public decimal? LuongCoBan1Ca { get; set; }
        public decimal? LuongTangCa1Gio { get; set; }

        public virtual CaLamViec MaCaLamViecNavigation { get; set; } = null!;
        public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
        public virtual ICollection<ChamCong> ChamCongs { get; set; }
        public virtual ICollection<DonHang> DonHangs { get; set; }
        public virtual ICollection<Luong> Luongs { get; set; }
        public virtual ICollection<PhieuNhap> PhieuNhaps { get; set; }
        public virtual ICollection<SaoLuuVaPhucHoi> SaoLuuVaPhucHois { get; set; }

        public string HoTen => $"{Ho} {Ten}";//new 27/10
    }
}
