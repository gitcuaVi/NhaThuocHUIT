namespace QuanLyNhaThuoc.Models
{
    public class ThongKeDonHangView
    {
        public int MaDonHang { get; set; }
        public int MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime NgayDatHang { get; set; }
        public DateTime? NgayCapNhat { get; set; }  // Nullable vì có thể không có giá trị
        public DateTime NgayGiaoHang { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
        public int Diem { get; set; }
    }
}
