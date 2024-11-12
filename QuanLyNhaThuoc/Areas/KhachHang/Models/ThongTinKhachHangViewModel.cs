namespace QuanLyNhaThuoc.Areas.KhachHang.Models
{
    public class ThongTinKhachHangViewModel
    {
        public int MaNguoiDung { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }

        // Thông tin từ bảng KhachHang
        public string TenKhachHang { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int Diem { get; set; }

    }
}
