using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class LoaiSanPham
    {
        public LoaiSanPham()
        {
            Thuocs = new HashSet<Thuoc>();
        }

        public int MaLoaiSanPham { get; set; }
        public string? TenLoai { get; set; }
        public int MaDanhMuc { get; set; }

        public virtual DanhMuc MaDanhMucNavigation { get; set; } = null!;
        public virtual ICollection<Thuoc> Thuocs { get; set; }
    }
}
