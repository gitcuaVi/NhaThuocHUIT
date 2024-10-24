namespace QuanLyNhaThuoc.Models
{
    public class ThongTinCaNhanView
    {
        public int MaNguoiDung { get; set; }
        public string TenNguoiDung { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string TrangThaiNguoiDung { get; set; }

        // Thông tin nhân viên (có thể null nếu không có)
        public int? MaNhanVien { get; set; }
        public string Ho { get; set; }
        public string TenNhanVien { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string ChucVu { get; set; }
        public DateTime? NgayTuyenDung { get; set; }
        public string TrangThaiNhanVien { get; set; }
        public decimal? LuongCoBan1Ca { get; set; }
        public decimal? LuongTangCa1Gio { get; set; }
    }

}
