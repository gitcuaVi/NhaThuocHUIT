namespace QuanLyNhaThuoc.ViewModels
{
    public class GioHangNhanVienThanhToanView
    {
        public int MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string HinhAnh { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public decimal TongTien => DonGia * SoLuong;
    }
}
