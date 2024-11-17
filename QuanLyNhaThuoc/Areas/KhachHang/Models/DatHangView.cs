namespace QuanLyNhaThuoc.Areas.KhachHang.Models
{
        public class DatHangView
        {
            public string HoTen { get; set; }
            public string SoDienThoai { get; set; }
            public string DiaChi { get; set; }
            public decimal TongTien { get; set; }
            public int MaDonHang { get; set; }
            public string NgayGiaoDuKien { get; set; }
            public List<ChiTietDonHangViewModel> ChiTietDonHang { get; set; }
        }

        public class ChiTietDonHangViewModel
        {
            public string TenThuoc { get; set; }
            public int SoLuong { get; set; }
            public decimal Gia { get; set; }
            public decimal ThanhTien { get; set; }
        }

}
