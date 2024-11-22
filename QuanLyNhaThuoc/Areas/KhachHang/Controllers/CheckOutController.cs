using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaThuoc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang/[controller]")]
    public class CheckOutController : Controller
    {
        private readonly QL_NhaThuocContext db;
        private readonly IMomoService _momoService;
        private readonly IOptions<MomoOptionModel> _options;  // Inject IOptions

        public CheckOutController(QL_NhaThuocContext context, IMomoService momoService, IOptions<MomoOptionModel> options)
        {
            db = context;
            _momoService = momoService;
            _options = options;  // Initialize _options
        }

        [HttpGet("PaymentCallBack")]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _momoService.PaymentExecuteMomo(HttpContext.Request.Query);
         var requestQuery = HttpContext.Request.Query;
            if (requestQuery["resultCode"]==0)
            {

            }   
            else
            {
                TempData["success"] = "Đã hủy giao dịch Momo";
                return RedirectToAction("Cart","SanPham");
            }

            return View(response);


        }
    }
}
