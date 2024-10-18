using System;
using System.Collections.Generic;

namespace QuanLyNhaThuoc.Models
{
    public partial class Faq
    {
        public int MaCauHoi { get; set; }
        public string? CauHoiThuongGap { get; set; }
        public string? CauTraLoiTuongUng { get; set; }
        public string? DanhMucCauHoi { get; set; }
        public DateTime? NgayTaoCauHoi { get; set; }
        public DateTime? NgayCapNhatCauHoi { get; set; }
    }
}
