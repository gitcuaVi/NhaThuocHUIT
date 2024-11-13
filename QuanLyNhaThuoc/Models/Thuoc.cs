using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class Thuoc
    {
        public Thuoc()
        {
            ChiTietDonHangs = new HashSet<ChiTietDonHang>();
            ChiTietPns = new HashSet<ChiTietPn>();
            ChiTietGioHangs = new HashSet<ChiTietGioHang>();
            HinhAnhs = new HashSet<HinhAnh>();
            TonKhos = new HashSet<TonKho>();
        }

        public int MaThuoc { get; set; }
        public string? TenThuoc { get; set; }
        public DateTime? HanSuDung { get; set; }
        public decimal DonGia { get; set; }
        public int? SoLuongTon { get; set; }
        public int? MaLoaiSanPham { get; set; }
        public string? DonVi { get; set; }

        public virtual LoaiSanPham? MaLoaiSanPhamNavigation { get; set; }
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public virtual ICollection<ChiTietPn> ChiTietPns { get; set; }
        public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; }
        public virtual ICollection<HinhAnh> HinhAnhs { get; set; }  
        public virtual ICollection<TonKho> TonKhos { get; set; }
    }
}
