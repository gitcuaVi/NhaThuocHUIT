namespace QuanLyNhaThuoc.Areas.KhachHang.Models
{
    public class ProductViewDetailsModel
    {
        public int MaThuoc { get; set; }
        public string TenDanhMuc { get; set; }
        public string TenThuoc { get; set; }
        public decimal DonGia { get; set; }
        public string DonVi { get; set; }
        public string? HinhAnh { get; set; }
        public DateTime? HanSuDung { get; set; }
        public int? SoLuongTon { get; set; } 
        public int MaDanhMuc { get; set; }
    }
}
