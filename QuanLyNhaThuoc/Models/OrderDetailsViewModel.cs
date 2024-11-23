using QuanLyNhaThuoc.Areas.KhachHang.Models;

namespace QuanLyNhaThuoc.Models
{
    public class OrderDetailsViewModel
    {
        public IEnumerable<ThongTinDatHangViewModel> OrderItems { get; set; } // Danh sách sản phẩm
        public PaymentModel PaymentInfo { get; set; } // Thông tin thanh toán
    }
}
