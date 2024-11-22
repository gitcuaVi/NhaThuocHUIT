using QuanLyNhaThuoc.Areas.KhachHang.Models;
namespace QuanLyNhaThuoc.Areas.KhachHang.Models
{
    public interface  IVnPayService
    {
        string CreatePaymentUrl(HttpContext context,PaymentRequestViewModel model );
        VnPaymentResponseModel PaymentExecute(IQueryCollection collection);
    }
}
