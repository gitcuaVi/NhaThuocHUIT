namespace QuanLyNhaThuoc.Areas.KhachHang.Models
{
    public class ProductViewModel
    {
        public int MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public decimal DonGia { get; set; }
        public string DonVi { get; set; }
        public string HinhAnhDauTien { get; set; }
        public int? SoLuongTon { get; set; }
    }
}