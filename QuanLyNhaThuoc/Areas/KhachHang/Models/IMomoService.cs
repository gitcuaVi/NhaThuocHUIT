namespace QuanLyNhaThuoc.Areas.KhachHang.Models
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentMomo(OrderInfo model);
        MomoExecuteResponseModel PaymentExecuteMomo(IQueryCollection collection);
        string ComputeHmacSha256(string message, string secretKey);
    }
}
