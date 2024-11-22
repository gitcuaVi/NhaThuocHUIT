namespace QuanLyNhaThuoc.Areas.KhachHang.Models
{
    public class LichsudonhangView
    {
        public int MaDonHang { get; set; }
        public decimal TongTien { get; set; }
        public DateTime NgayDatHang { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? NgayGiaoHang { get; set; }
        public string TrangThai { get; set; }
        public string? TenKhachHang { get; set; }  
        public string? TenThuoc { get; set; }      
        public int SoLuong { get; set; }          
        public decimal Gia { get; set; }          
        public decimal ThanhTien { get; set; }
        public int MaKhachHang { get; set; }
        public string? HinhAnhDauTien { get; set; }
        public int MaThuoc { get; set; }
    }
}
