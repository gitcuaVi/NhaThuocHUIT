namespace QuanLyNhaThuoc.ViewModels
{
    public class DonHangViewModel
    {
        public int MaDonHang { get; set; }
        public decimal TongTien { get; set; }
        public DateTime NgayDatHang { get; set; }
        public string TrangThai { get; set; }
        public List<ChiTietDonHangNhanVienViewModel> ChiTietDonHang { get; set; }
    }
}


namespace QuanLyNhaThuoc.ViewModels
{
    public class ChiTietDonHangNhanVienViewModel
    {
        public string TenThuoc { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
    }
}

