using QuanLyNhaThuoc.Models;

namespace QuanLyNhaThuoc.ViewModels
{
    public class ThongTinDonHangViewModel
    {
        // Thông tin khách hàng
        public string TenKhachHang { get; set; } = null!;
        public string SoDienThoai { get; set; } = null!;
        public string GioiTinh { get; set; } = null!;

        // Địa chỉ giao hàng (nếu cần nhập thêm)
        public string DiaChi { get; set; } = "Tại nhà thuốc";

        // Chi tiết giỏ hàng
        public List<CartItem> GioHang { get; set; } = new();
    }
}
