namespace QuanLyNhaThuoc.Models
{
    public class DonHangDetailsViewModel
    {
        public int MaDonHang { get; set; }
        public DateTime NgayDatHang { get; set; }
        public string? DiaChi { get; set; }
        public string? TrangThai { get; set; }
        public decimal TongTien { get; set; }

        // Danh sách các chi tiết đơn hàng
        public List<ChiTietDonHangViewModel> ChiTietDonHangs { get; set; }
    }



}
