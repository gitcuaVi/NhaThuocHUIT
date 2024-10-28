using QuanLyNhaThuoc.Models;

public class PhieuNhapViewModel
{
    public int MaNhanVien { get; set; }
    public decimal TongTien { get; set; }
    public DateTime NgayNhap { get; set; }
    public string? GhiChu { get; set; }
    public string NhaCungCap { get; set; }
    public List<ChiTietPn> ChiTietPns { get; set; }
}
