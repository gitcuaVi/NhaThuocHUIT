using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class PhanQuyen
    {
        public int? MaVaiTro { get; set; }
        public int? MaQuyen { get; set; }

        public virtual QuyenTruyCap? MaQuyenNavigation { get; set; }
        public virtual VaiTro? MaVaiTroNavigation { get; set; }
    }
}
