using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class TonKho
    {
        public TonKho()
        {
            ChiTietPns = new HashSet<ChiTietPn>();
        }

        public int MaTonKho { get; set; }
        public int SoLuongTon { get; set; }
        public int SoLuongCanhBao { get; set; }
        public int SoLuongHienTai { get; set; }
        public int SoLuongToiDa { get; set; }
        public string? TrangThai { get; set; }
        public DateTime? NgayGioCapNhat { get; set; }
        public int MaThuoc { get; set; }

        public virtual Thuoc MaThuocNavigation { get; set; } = null!;
        public virtual ICollection<ChiTietPn> ChiTietPns { get; set; }
    }
}
