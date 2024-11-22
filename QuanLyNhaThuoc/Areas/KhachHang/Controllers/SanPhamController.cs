using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuanLyNhaThuoc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Newtonsoft.Json;

namespace QuanLyNhaThuoc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang/[controller]")]
    public class SanPhamController : Controller
    {
        private readonly QL_NhaThuocContext db;
        private readonly IVnPayService _vnPayservice;
        private readonly IMomoService _momoService;


        public SanPhamController(QL_NhaThuocContext context,IVnPayService vnPayservice, IMomoService momoService)
        {
            db = context;
            _vnPayservice = vnPayservice;
            _momoService = momoService;

        }

        [HttpGet]
        public async Task<IActionResult> Index(int categoryId, decimal? minPrice, decimal? maxPrice, string productType)
        {
            var paramCategoryId = new SqlParameter("@MaDanhMuc", categoryId);
            var paramMinPrice = new SqlParameter("@MinPrice", minPrice ?? (object)DBNull.Value);
            var paramMaxPrice = new SqlParameter("@MaxPrice", maxPrice ?? (object)DBNull.Value);
            var paramProductType = new SqlParameter("@ProductType", string.IsNullOrEmpty(productType) ? (object)DBNull.Value : productType);

            var products = await db.Set<ProductViewModel>()
                .FromSqlRaw("EXEC sp_GetAllProductsByCategoryAndFilters @MaDanhMuc, @MinPrice, @MaxPrice, @ProductType",
                            paramCategoryId, paramMinPrice, paramMaxPrice, paramProductType)
                .ToListAsync();

            var productTypes = await db.LoaiSanPhams
                .Where(l => l.MaDanhMuc == categoryId)
                .ToListAsync();

            // Lấy thông tin danh mục
            var category = await db.DanhMucs
                .FirstOrDefaultAsync(d => d.MaDanhMuc == categoryId);

            if (category == null)
            {
                return NotFound("Danh mục không tồn tại.");
            }

            ViewData["ProductTypes"] = productTypes;

            ViewData["Products"] = products;

            ViewData["CategoryName"] = category?.TenDanhMuc ?? "Danh mục không xác định";

            return View();
        }

        private int GetMaKhachHangFromClaims()
        {
            var maKhachHangClaim = User.FindFirst("MaKhachHang")?.Value;
            return int.TryParse(maKhachHangClaim, out var maKhachHang) ? maKhachHang : -1;
        }

        [HttpGet("Chitietsp/{id}")]
        public async Task<IActionResult> Chitietsp(int id)
        {
            var paramId = new SqlParameter("@MaThuoc", id);
            var productDetail = await db.Set<ProductViewDetailsModel>()
                .FromSqlRaw("SELECT * FROM vw_ChiTietThuoc WHERE MaThuoc = @MaThuoc", paramId)
                .FirstOrDefaultAsync();

            if (productDetail == null)
            {
                return NotFound();
            }

            return View(productDetail);

        }


        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(int maThuoc, int soLuong)
        {
            int maKhachHang = GetMaKhachHangFromClaims();
            if (maKhachHang == -1)
            {
                return Json(new { success = false, redirectToLogin = true, loginUrl = Url.Action("Login", "UserDH") });
            }

            try
            {
                await db.Database.ExecuteSqlRawAsync("EXEC sp_AddToCart @MaKhachHang, @MaThuoc, @SoLuong",
                    new SqlParameter("@MaKhachHang", maKhachHang),
                    new SqlParameter("@MaThuoc", maThuoc),
                    new SqlParameter("@SoLuong", soLuong)
                );

                return Json(new { success = true, message = "Thêm vào giỏ hàng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [HttpGet("Cart")]
        public async Task<IActionResult> Cart()
        {
            int maKhachHang = GetMaKhachHangFromClaims();
            if (maKhachHang == -1)
            {
                return RedirectToAction("Login", "UserDH");
            }

            var paramMaKhachHang = new SqlParameter("@MaKhachHang", maKhachHang);

            // Lấy thông tin khách hàng
            var customerInfo = db.Set<ThongTinKhachHangGioHang>()
                .FromSqlRaw("EXEC sp_GetThongTinKhachHangGioHang @MaKhachHang", paramMaKhachHang)
                .AsEnumerable() 
                .FirstOrDefault(); 

            if (customerInfo != null)
            {
                ViewBag.HoTen = customerInfo.TenKhachHang;
                ViewBag.SoDienThoai = customerInfo.SoDienThoai;
                ViewBag.DiaChi = customerInfo.DiaChi;
            }

            // Lấy số lượng sản phẩm trong giỏ
            var cartCount = await db.Database
                .ExecuteSqlRawAsync("EXEC sp_GetCartCountByKhachHang @MaKhachHang", paramMaKhachHang);

            ViewData["CartCount"] = cartCount;

            // Lấy chi tiết giỏ hàng
            var cartItems = await db.Set<GioHangViewModel>()
                .FromSqlRaw("EXEC sp_GetGioHangByKhachHang @MaKhachHang", paramMaKhachHang)
                .ToListAsync();

            return View(cartItems);
        }




        [HttpGet("GetCartCount")]
        public async Task<IActionResult> GetCartCount()
        {
            int maKhachHang = GetMaKhachHangFromClaims();
            if (maKhachHang == -1)
            {
                return Json(new { count = 0 });
            }

            // Lấy giỏ hàng của khách hàng hiện tại
            var gioHang = await db.GioHangs
                .FirstOrDefaultAsync(g => g.MaKhachHang == maKhachHang);

            if (gioHang == null)
            {
                return Json(new { count = 0 });
            }

            var cartCount = await db.ChiTietGioHangs
                .Where(c => c.MaGioHang == gioHang.MaGioHang)
                .SumAsync(c => c.SoLuong);

            return Json(new { count = cartCount });
        }

        [HttpPost("UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityModel model)
        {
            int maKhachHang = GetMaKhachHangFromClaims();
            if (model == null || model.MaChiTietGioHang <= 0 || model.SoLuong <= 0)
            {
                Console.WriteLine("Invalid data: " + model);
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }


            try
            {
                var paramMaChiTietGioHang = new SqlParameter("@MaChiTietGioHang", model.MaChiTietGioHang);
                var paramSoLuong = new SqlParameter("@SoLuong", model.SoLuong);

                await db.Database.ExecuteSqlRawAsync("EXEC sp_UpdateCartItemQuantity @MaChiTietGioHang, @SoLuong", paramMaChiTietGioHang, paramSoLuong);

                return Json(new { success = true, message = "Cập nhật số lượng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart([FromBody] int maChiTietGioHang)
        {
            int maKhachHang = GetMaKhachHangFromClaims();
            try
            {
                var paramMaChiTietGioHang = new SqlParameter("@MaChiTietGioHang", maChiTietGioHang);

                int rowsAffected = await db.Database.ExecuteSqlRawAsync("EXEC sp_RemoveFromCart @MaChiTietGioHang", paramMaChiTietGioHang);

                if (rowsAffected > 0)
                    return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
                else
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm để xóa." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }


        [HttpPost("DatHang")]
        public async Task<IActionResult> DatHang(string diaChi)
        {
            int maKhachHang = GetMaKhachHangFromClaims();
            if (maKhachHang == -1)
            {
                return RedirectToAction("Login", "UserDH");
            }

            try
            {
                var outputParam = new SqlParameter
                {
                    ParameterName = "@MaDonHangMoi",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };

                await db.Database.ExecuteSqlRawAsync(
                    "EXEC sp_DatHang @MaKhachHang, @DiaChi, @MaDonHangMoi OUTPUT",
                    new SqlParameter("@MaKhachHang", maKhachHang),
                    new SqlParameter("@DiaChi", diaChi),
                    outputParam
                );

                int maDonHangMoi = (int)(outputParam.Value ?? 0);

                if (maDonHangMoi == 0)
                {
                    throw new Exception("Không thể tạo đơn hàng.");
                }

                return RedirectToAction("OrderDetails", new { maDonHang = maDonHangMoi });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi khi đặt hàng.", details = ex.Message });
            }
        }

        [HttpGet("OrderDetails/{maDonHang}")]
        public async Task<IActionResult> OrderDetails(int maDonHang)
        {
            try
            {
                var orderDetails = await db.ThongTinDatHangViewModels.FromSqlRaw(
                    "EXEC sp_GetThongTinDatHang @MaDonHang",
                    new SqlParameter("@MaDonHang", maDonHang)
                ).ToListAsync();

                if (!orderDetails.Any())
                {
                    return NotFound("Không tìm thấy thông tin đơn hàng.");
                }

                return View(orderDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi: " + ex.Message);
            }
        }
        [HttpPost("SubmitPayment")]
        public async Task<IActionResult> SubmitPayment([FromForm] PaymentRequestModel model)
        {
            if (model == null || model.MaDonHang <= 0 || string.IsNullOrEmpty(model.PaymentMethod))
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            try
            {
                // Lấy thông tin đặt hàng từ cơ sở dữ liệu
                var paramMaDonHang = new SqlParameter("@MaDonHang", model.MaDonHang);
                var orderDetails = await db.ThongTinDatHangViewModels.FromSqlRaw(
                    "EXEC sp_GetThongTinDatHang @MaDonHang",
                    paramMaDonHang
                ).ToListAsync();

                if (!orderDetails.Any())
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin đơn hàng." });
                }

                // Cập nhật phương thức thanh toán
                var paramPhuongThucThanhToan = new SqlParameter("@PhuongThucThanhToan", model.PaymentMethod);
                await db.Database.ExecuteSqlRawAsync(
                    "EXEC [dbo].[CapNhatPhuongThucThanhToan] @MaDonHang, @PhuongThucThanhToan",
                    paramMaDonHang, paramPhuongThucThanhToan
                );

                // Nếu chọn phương thức QR Momo
                if (model.PaymentMethod == "qr-momo")
                {
                    var orderInfo = new OrderInfo
                    {
                        Fullname = orderDetails.First().TenKhachHang,
                        Amount = (double)orderDetails.Sum(x => x.ThanhTien), // Ép kiểu từ decimal sang double
                        OrderInfomation = "Thanh toán đơn hàng thuốc",
                        OrderId = model.MaDonHang.ToString()
                    };

                    // Gọi hàm CreatePaymentUrl để tạo URL thanh toán MoMo
                    return await CreatePaymentUrl(orderInfo);
                }

                // Xử lý các phương thức thanh toán khác
                return Json(new { success = true, message = "Thanh toán cập nhật thành công.", maDonHang = model.MaDonHang });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        [HttpPost("MomoNotify")]
        public async Task<IActionResult> MomoNotify([FromBody] MomoExecuteResponseModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.OrderId))
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                int maDonHang = int.Parse(model.OrderId);

                // Gọi stored procedure cập nhật trạng thái thanh toán
                await db.Database.ExecuteSqlRawAsync(
                    "EXEC sp_CapNhatTrangThaiThanhToan @MaDonHang",
                    new SqlParameter("@MaDonHang", maDonHang)
                );

                // Truyền thông báo thành công và mã đơn hàng qua TempData
                TempData["Message"] = "Thanh toán MoMo thành công!";
                TempData["MaDonHang"] = maDonHang;

                // Chuyển hướng đến view MomoPaymentResult
                return RedirectToAction("MomoPaymentResult");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Đã xảy ra lỗi: " + ex.Message;
                return RedirectToAction("Cart");
            }
        }
     
        [HttpPost]
        [Route("CreatePaymentUrl")]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfo model)
        {
            var response = await _momoService.CreatePaymentMomo(model);

            if (!string.IsNullOrEmpty(response.PayUrl))
            {
                return Redirect(response.PayUrl); // Chuyển hướng tới URL thanh toán MoMo
            }

            return Json(new { success = false, message = "Không thể tạo URL thanh toán Momo." });
        }

    }
}


