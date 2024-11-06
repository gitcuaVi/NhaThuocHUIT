namespace QuanLyNhaThuoc.Models
{
    public class ChiTietDonHangViewModel
    {
        public int MaThuoc { get; set; }
        public string? TenThuoc { get; set; }  // Tên thuốc

        public string? DonVi { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public decimal ThanhTien { get; set; }  // Thành tiền (SoLuong * Gia)
    }
}
