using QuanLyNhaThuoc.Models;

namespace QuanLyNhaThuoc.Areas.KhachHang.Models
{
    public class ReportOrder
    {
        public string code { get; set; }
         public   DateTime CreateOn { get; set; }
        public OrderAddess DiaChi { get; set; } // Địa chỉ giao hàng
        public DateTime NgayDatHang { get; set; }
        public IEnumerable<OrderDetail> Details { get; set; }
        public decimal TongTien; // Tính Thành tiền

    }
    public class OrderAddess
    {
        public string TenKhachHang { get; set; } // Tên khách hàng
        public string SoDienThoai { get; set; } // Số điện thoại khách hàng
        public string DiaChi { get; set; } // Địa chỉ giao hàng
    }
    public class OrderDetail
    {
        public string TenSanPham { get; set; } // Tên sản phẩm
        public int SoLuong { get; set; } // Số lượng sản phẩm
        public decimal DonGia { get; set; } // Đơn giá sản phẩm
        public decimal ThanhTien;
    
    }
}
