using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaThuoc.Areas.KhachHang.Models
{
    public class RegisterViewModel
    {
        public string TenKhachHang { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        public string GioiTinh { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không khớp")]
        public string ConfirmPassword { get; set; }
    }
}
