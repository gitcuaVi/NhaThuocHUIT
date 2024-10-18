using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class CaLamViec
    {
        public CaLamViec()
        {
            NhanViens = new HashSet<NhanVien>();
        }

        public int MaCaLam { get; set; }
        public TimeSpan? ThoiGianBatDau { get; set; }
        public TimeSpan? ThoiGianKetThuc { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public string? GhiChuCongViec { get; set; }
        public TimeSpan? GioNghiTrua { get; set; }

        public virtual ICollection<NhanVien> NhanViens { get; set; }
    }
}
