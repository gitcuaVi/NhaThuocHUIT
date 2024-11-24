using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.Areas.KhachHang.Models;

namespace QuanLyNhaThuoc.Areas.KhachHang.Services.Momo
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentMomo(OrderInfo model);
        MomoExecuteResponseModel PaymentExecuteMomo(IQueryCollection collection);
    }
}
