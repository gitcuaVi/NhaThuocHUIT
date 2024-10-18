using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class VaiTro
    {
        public VaiTro()
        {
            NguoiDungs = new HashSet<NguoiDung>();
        }

        public int MaVaiTro { get; set; }
        public string TenVaiTro { get; set; } = null!;

        public virtual ICollection<NguoiDung> NguoiDungs { get; set; }
    }
}
