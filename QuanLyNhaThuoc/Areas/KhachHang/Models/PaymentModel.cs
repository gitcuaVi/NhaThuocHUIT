namespace QuanLyNhaThuoc.Areas.KhachHang.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }  // Khóa chính
        public int MaDonHang { get; set; } // ID đơn hàng
        public string PaymentMethod { get; set; } // Phương thức thanh toán
        public decimal TotalAmount { get; set; } // Tổng
    }
}
