using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class DanhMuc
    {
        public DanhMuc()
        {
            LoaiSanPhams = new HashSet<LoaiSanPham>();
        }

        public int MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; } = null!;
        public int LoaiMenu { get; set; }

        public virtual ICollection<LoaiSanPham> LoaiSanPhams { get; set; }
    }   
}
