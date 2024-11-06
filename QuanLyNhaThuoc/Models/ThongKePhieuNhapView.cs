using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaThuoc.Models
{
    public class ThongKePhieuNhapView
    {
      
        public int MaPhieuNhap { get; set; }

        public int MaNhanVien { get; set; }

        public decimal TongTien { get; set; }

        [DataType(DataType.Date)]
        public DateTime NgayNhap { get; set; }

        public string GhiChu { get; set; }

        public string NhaCungCap { get; set; }
    }
}
