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
using QuanLyNhaThuoc.Areas.KhachHang.Services.VnPay;
using QuanLyNhaThuoc.Areas.KhachHang.Models.VnPay;
using QuanLyNhaThuoc.KhachHang.Services.VnPay;
using System.Threading.Tasks;
using QuanLyNhaThuoc.Areas.KhachHang.Services;
using QuanLyNhaThuoc.Areas.KhachHang.Services.Momo;
using System.Data;

namespace QuanLyNhaThuoc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang/[controller]")]
    public class SanPhamController : Controller
    {
        private readonly QL_NhaThuocContext db;
        private readonly IMomoService _momoService;
        private readonly IVnPayService _vnPayService;


        public SanPhamController(QL_NhaThuocContext context, IVnPayService vnPayService, IMomoService momoService)
        {
            db = context;
            _vnPayService = vnPayService;
            _momoService = momoService;
          
        }
        [HttpGet]
        public async Task<IActionResult> Index(int categoryId, string categoryName, decimal? minPrice, decimal? maxPrice, string productType)
        {
            var paramCategoryId = new SqlParameter("@MaDanhMuc", categoryId);
            var paramMinPrice = new SqlParameter("@MinPrice", minPrice ?? (object)DBNull.Value);
            var paramMaxPrice = new SqlParameter("@MaxPrice", maxPrice ?? (object)DBNull.Value);
            var paramProductType = new SqlParameter("@ProductType",
                string.IsNullOrEmpty(productType) ? (object)DBNull.Value : productType);

            IEnumerable<ProductViewModel> products;

            if (categoryName.StartsWith("Xem tất cả"))
            {
                var loaiMenu = await GetLoaiMenuByCategoryId(categoryId);

                if (loaiMenu > 0)
                {
                    var paramLoaiMenu = new SqlParameter("@LoaiMenu", loaiMenu);
                    //lấy theo loại menu
                    products = await db.Set<ProductViewModel>()
                        .FromSqlRaw("EXEC sp_GetAllProductsByCategoryAndMenu @LoaiMenu, @MinPrice, @MaxPrice, @ProductType",
                                    paramLoaiMenu, paramMinPrice, paramMaxPrice, paramProductType)
                        .ToListAsync();
                }
                else
                {
                    return NotFound("Loại menu không hợp lệ.");
                }
            }
            else
            {
                // Lấy sản phẩm theo danh mục 
                products = await db.Set<ProductViewModel>()
                    .FromSqlRaw("EXEC sp_GetAllProductsByCategoryAndFilters @MaDanhMuc, @MinPrice, @MaxPrice, @ProductType",
                                paramCategoryId, paramMinPrice, paramMaxPrice, paramProductType)
                    .ToListAsync();
            }

            // Lấy các loại sản phẩm cho danh mục
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
            ViewData["CategoryId"] = categoryId;

            return View();
        }

        public async Task<int?> GetLoaiMenuByCategoryId(int categoryId)
        {
            //  lấy giá trị LoaiMenu theo MaDanhMuc
            var category = await db.DanhMucs
                                   .Where(d => d.MaDanhMuc == categoryId)
                                   .Select(d => d.LoaiMenu)  // Chỉ lấy trường LoaiMenu
                                   .FirstOrDefaultAsync();

            //trả về giá trị LoaiMenu, không thì trả về null
            return category;
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

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string keyword)
        {
            var paramKeyword = new SqlParameter("@Keyword", string.IsNullOrEmpty(keyword) ? (object)DBNull.Value : keyword);

            var products = await db.Set<ProductViewModel>()
                .FromSqlRaw("EXEC sp_SearchThuocByTenThuoc @Keyword", paramKeyword)
                .ToListAsync();

            return PartialView("_SearchResults", products);
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
                ViewBag.GhiChu = customerInfo.GhiChu;
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
        public async Task<IActionResult> DatHang(string diaChi, string ghiChu)
        {
            int maKhachHang = GetMaKhachHangFromClaims();
            if (maKhachHang == -1)
            {
                return RedirectToAction("Login", "UserDH");
            }

            try
            {
                // Lấy giỏ hàng
                var gioHangItems = await db.Set<GioHangViewModel>()
                    .FromSqlRaw("EXEC sp_GetGioHangByKhachHang @MaKhachHang", new SqlParameter("@MaKhachHang", maKhachHang))
                    .ToListAsync();

                if (gioHangItems == null || !gioHangItems.Any())
                {
                    return Json(new { success = false, message = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi đặt hàng!" });
                }

                var gioHangTable = new DataTable();
                gioHangTable.Columns.Add("MaThuoc", typeof(int));
                gioHangTable.Columns.Add("SoLuong", typeof(int));

                foreach (var item in gioHangItems)
                {
                    gioHangTable.Rows.Add(item.MaThuoc, item.SoLuong);
                }

                var gioHangParam = new SqlParameter
                {
                    ParameterName = "@GioHang",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.GioHangTableType",
                    Value = gioHangTable
                };

                var outputParam = new SqlParameter
                {
                    ParameterName = "@MaDonHangMoi",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                await db.Database.ExecuteSqlRawAsync(
                    "EXEC sp_DatHang @MaKhachHang, @DiaChi, @GhiChu, @GioHang, @MaDonHangMoi OUTPUT",
                    new SqlParameter("@MaKhachHang", maKhachHang),
                    new SqlParameter("@DiaChi", diaChi),
                    new SqlParameter("@GhiChu", ghiChu ?? string.Empty),
                    gioHangParam,
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
                return Json(new { success = false, message = "Đặt hàng không thành công!" });
            }
        }

        [HttpGet("OrderDetails/{maDonHang}")]
        public async Task<IActionResult> OrderDetails(int maDonHang)
        {
            try
            {
                // Lấy thông tin chi tiết đơn hàng từ stored procedure
                var orderDetails = await db.ThongTinDatHangViewModels.FromSqlRaw(
                    "EXEC sp_GetThongTinDatHang @MaDonHang",
                    new SqlParameter("@MaDonHang", maDonHang)
                ).ToListAsync();

                if (!orderDetails.Any())
                {
                    return NotFound("Không tìm thấy thông tin đơn hàng.");
                }

                // Tạo PaymentModel
                var paymentModel = new PaymentModel
                {
                    MaDonHang = maDonHang,
                    TotalAmount = orderDetails.Sum(x => x.ThanhTien),
                    PaymentMethod = null // Người dùng sẽ chọn
                };

                // Tạo ViewModel tổng hợp
                var viewModel = new OrderDetailsViewModel
                {
                    OrderItems = orderDetails,
                    PaymentInfo = paymentModel
                };

                return View(viewModel); // Truyền ViewModel tổng hợp
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
                // Lấy thông tin đơn hàng từ stored procedure
                var paramMaDonHang = new SqlParameter("@MaDonHang", model.MaDonHang);
                var orderDetails = await db.ThongTinDatHangViewModels
                                            .FromSqlRaw("EXEC sp_GetThongTinDatHang @MaDonHang", paramMaDonHang)
                                            .ToListAsync();

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
                TempData["Message"] = "Phương thức thanh toán đã được cập nhật thành công.";
                if (model.PaymentMethod == "qr-momo")
                {
                    // Tạo thông tin thanh toán Momo
                    var orderInfo = new OrderInfo
                    {
                        Fullname = orderDetails.First().TenKhachHang,
                        Amount = (double)orderDetails.Sum(x => x.ThanhTien),
                        OrderInfomation = "Thanh toán đơn hàng thuốc",
                        OrderId = model.MaDonHang.ToString(),
                        OrderIdOld = model.MaDonHang.ToString() // Set OrderIdOld to the same value as MaDonHang
                    };
                    return await CreatePaymentUrl(orderInfo); // Gọi phương thức xử lý Momo

                }

                if (model.PaymentMethod == "qr-vnpay")
                {
                    // Tạo thông tin thanh toán VNPAY
                    var paymentInfo = new PaymentInformationModel
                    {
                        Name = orderDetails.First().TenKhachHang,
                        Amount = (double)orderDetails.Sum(x => x.ThanhTien),
                        OrderDescription = "Thanh toán đơn hàng thuốc",
                        OrderType = "other",
                        OrderId = model.MaDonHang.ToString() // Truyền OrderId vào model
                    };

                    // Tạo URL thanh toán qua VNPAY
                    var paymentUrl = _vnPayService.CreatePaymentUrl(paymentInfo, HttpContext);

                    // Gọi stored procedure để cập nhật mã giao dịch trong bảng ThanhToan
                    var paramMaGiaoDich = new SqlParameter("@MaGiaoDich", paymentInfo.OrderId); // Giả sử OrderId là mã giao dịch từ VNPAY
                    await db.Database.ExecuteSqlRawAsync(
                        "EXEC [dbo].[sp_UpdateMaGiaoDich] @MaDonHang, @MaGiaoDich",
                        paramMaDonHang, paramMaGiaoDich
                    );

                    // Redirect to payment URL first
                    return Redirect(paymentUrl);
                }
                return RedirectToAction("OrderDetails", "SanPham", new { maDonHang = model.MaDonHang });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

     
    

        [HttpPost]
        [Route("CreatePaymentUrl")]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfo model)
        {
            var response = await _momoService.CreatePaymentMomo(model);

            if (!string.IsNullOrEmpty(response.PayUrl))
            {
                return Redirect(response.PayUrl);
            }

            return Json(new { success = false, message = "Không thể tạo URL thanh toán Momo." });
        }
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
       
    }
}


