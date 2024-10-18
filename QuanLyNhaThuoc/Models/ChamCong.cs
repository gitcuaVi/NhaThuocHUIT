using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class ChamCong
    {
        public int MaChamCong { get; set; }
        public int MaNhanVien { get; set; }
        public DateTime? ThoiGianVaoLam { get; set; }
        public DateTime? ThoiGianRaVe { get; set; }
        public DateTime NgayChamCong { get; set; }
        public string GhiChu { get; set; } = null!;

        public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;
    }
}
