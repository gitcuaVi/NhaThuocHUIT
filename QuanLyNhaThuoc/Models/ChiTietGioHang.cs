namespace QuanLyNhaThuoc.Models
{
    public class ChiTietGioHang
    {

        public int MaChiTietGioHang { get; set; }
        public int MaGioHang { get; set; }
        public int MaThuoc { get; set; }
        public int SoLuong { get; set; }

        public virtual GioHang GioHangNavigation { get; set; } = null!;
        public virtual Thuoc Thuoc { get; set; } = null!;

    }

}
