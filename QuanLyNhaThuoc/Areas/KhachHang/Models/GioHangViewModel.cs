namespace QuanLyNhaThuoc.Areas.KhachHang.Models
{
    public class GioHangViewModel
    {
        public int MaChiTietGioHang { get; set; }
        public int MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string HinhAnhDauTien { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public string DonVi { get; set; }
        public decimal ThanhTien => DonGia * SoLuong;

    }
}
