namespace QuanLyNhaThuoc.Areas.KhachHang.Models
{
    public class ThongTinDatHangViewModel
    {
        public int MaDonHang { get; set; } 
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public string TenSanPham { get; set; }
        public string TrangThaiThanhToan { get; set; }

        // Thuộc tính Thành tiền
        public decimal ThanhTien => SoLuong * DonGia; // Tính Thành tiền
    }
}
