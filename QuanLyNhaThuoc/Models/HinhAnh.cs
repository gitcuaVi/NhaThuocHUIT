using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class HinhAnh
    {
        public int MaHinh { get; set; }
        public string? UrlAnh { get; set; }
        public int? MaThuoc { get; set; }

        public virtual Thuoc? MaThuocNavigation { get; set; }
    }
}
