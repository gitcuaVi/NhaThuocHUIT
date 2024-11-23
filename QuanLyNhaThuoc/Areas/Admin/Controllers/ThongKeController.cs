using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuanLyNhaThuoc.Models;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class ThongKeController : Controller
    {
        private readonly QL_NhaThuocContext _context;

        public ThongKeController(QL_NhaThuocContext context)
        {
            _context = context;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Thiết lập giấy phép sử dụng EPPlus
        }
        [HttpGet]
        // Trang chính
        public async Task<IActionResult> Index()
        {
        
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> IndexDonHang()
        {
            var danhSachDonHang = await _context.vw_ThongKeDonHang.ToListAsync();
            return View(danhSachDonHang);
        }
        [HttpGet("Loc")]
        public async Task<IActionResult> IndexDonHang(int? selectedMonth, int? selectedYearForMonth, int? selectedQuarter, int? selectedYearForQuarter, int? selectedYear)
        {

            var query = _context.vw_ThongKeDonHang.AsQueryable();



            // Lọc theo tháng và năm
            if (selectedMonth.HasValue && selectedYearForMonth.HasValue)
            {
                query = query.Where(dh => dh.NgayGiaoHang.Month == selectedMonth.Value && dh.NgayGiaoHang.Year == selectedYearForMonth.Value);
            }
            // Lọc theo quý và năm
            else if (selectedQuarter.HasValue && selectedYearForQuarter.HasValue)
            {
                int startMonth = (selectedQuarter.Value - 1) * 3 + 1;
                int endMonth = startMonth + 2;
                query = query.Where(dh => dh.NgayGiaoHang.Year == selectedYearForQuarter.Value
                                           && dh.NgayGiaoHang.Month >= startMonth
                                           && dh.NgayGiaoHang.Month <= endMonth);
            }
            // Lọc theo năm
            else if (selectedYear.HasValue)
            {
                query = query.Where(dh => dh.NgayGiaoHang.Year == selectedYear.Value);
            }

            // Tính tổng tiền các đơn hàng sau khi lọc
            var danhSachDonHang = await query.ToListAsync();
            var tongTien = danhSachDonHang.Sum(dh => dh.TongTien);

            ViewBag.TongTien = tongTien;
            return View(danhSachDonHang);


        }
        [HttpGet("ExportToExcel")]
        public async Task<IActionResult> ExportToExcel(int? selectedMonth, int? selectedYearForMonth, int? selectedQuarter, int? selectedYearForQuarter, int? selectedYear)
        {
            // Lọc theo tháng và năm
            var query = _context.vw_ThongKeDonHang.AsQueryable();

            if (selectedMonth.HasValue && selectedYearForMonth.HasValue)
            {
                query = query.Where(dh => dh.NgayGiaoHang.Month == selectedMonth.Value && dh.NgayGiaoHang.Year == selectedYearForMonth.Value);
            }
            // Lọc theo quý và năm
            else if (selectedQuarter.HasValue && selectedYearForQuarter.HasValue)
            {
                int startMonth = (selectedQuarter.Value - 1) * 3 + 1;
                int endMonth = startMonth + 2;
                query = query.Where(dh => dh.NgayGiaoHang.Year == selectedYearForQuarter.Value
                                           && dh.NgayGiaoHang.Month >= startMonth
                                           && dh.NgayGiaoHang.Month <= endMonth);
            }
            // Lọc theo năm
            else if (selectedYear.HasValue)
            {
                query = query.Where(dh => dh.NgayGiaoHang.Year == selectedYear.Value);
            }

            // Lấy danh sách đơn hàng
            var danhSachDonHang = await query.ToListAsync();

            // Tính tổng tiền
            var tongTien = danhSachDonHang.Sum(dh => dh.TongTien);

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("ThongKeDonHang");
                worksheet.Cells[1, 1].Value = "Mã Đơn Hàng";
                worksheet.Cells[1, 2].Value = "Tên Khách Hàng";
                worksheet.Cells[1, 3].Value = "Ngày Đặt Hàng";
                worksheet.Cells[1, 4].Value = "Ngày Giao Hàng";
                worksheet.Cells[1, 5].Value = "Tổng Tiền";

                for (int i = 0; i < danhSachDonHang.Count; i++)
                {
                    var donHang = danhSachDonHang[i];
                    worksheet.Cells[i + 2, 1].Value = donHang.MaDonHang;
                    worksheet.Cells[i + 2, 2].Value = donHang.TenKhachHang;
                    worksheet.Cells[i + 2, 3].Value = donHang.NgayDatHang.ToString("dd/MM/yyyy");
                    worksheet.Cells[i + 2, 4].Value = donHang.NgayGiaoHang.ToString("dd/MM/yyyy");
                    worksheet.Cells[i + 2, 5].Value = donHang.TongTien;
                }

                // Thêm hàng tổng cộng
                worksheet.Cells[danhSachDonHang.Count + 2, 4].Value = "Tổng cộng:";
                worksheet.Cells[danhSachDonHang.Count + 2, 5].Value = tongTien;

                // Định dạng cho hàng tổng cộng
                using (var range = worksheet.Cells[danhSachDonHang.Count + 2, 4, danhSachDonHang.Count + 2, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                worksheet.Cells.AutoFitColumns(); // Tự động điều chỉnh kích thước cột

                // Trả về file Excel
                var fileDownloadName = "ThongKeDonHang.xlsx";
                var fileBytes = package.GetAsByteArray();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileDownloadName);
            }
        }
  

            [HttpPost]
            public async Task<IActionResult> IndexPhieuNhap()
            {
                var danhSachPhieuNhap = await _context.vw_ThongKePhieuNhap.ToListAsync();
                return View(danhSachPhieuNhap);
            }

            [HttpGet("LocPhieuNhap")]
            public async Task<IActionResult> IndexPhieuNhap(int? selectedMonth, int? selectedYearForMonth, int? selectedQuarter, int? selectedYearForQuarter, int? selectedYear)
            {
                var query = _context.vw_ThongKePhieuNhap.AsQueryable();

                // Lọc theo tháng và năm
                if (selectedMonth.HasValue && selectedYearForMonth.HasValue)
                {
                    query = query.Where(pn => pn.NgayNhap.Month == selectedMonth.Value && pn.NgayNhap.Year == selectedYearForMonth.Value);
                }
                // Lọc theo quý và năm
                else if (selectedQuarter.HasValue && selectedYearForQuarter.HasValue)
                {
                    int startMonth = (selectedQuarter.Value - 1) * 3 + 1;
                    int endMonth = startMonth + 2;
                    query = query.Where(pn => pn.NgayNhap.Year == selectedYearForQuarter.Value
                                               && pn.NgayNhap.Month >= startMonth
                                               && pn.NgayNhap.Month <= endMonth);
                }
                // Lọc theo năm
                else if (selectedYear.HasValue)
                {
                    query = query.Where(pn => pn.NgayNhap.Year == selectedYear.Value);
                }

                // Tính tổng tiền các phiếu nhập sau khi lọc
                var danhSachPhieuNhap = await query.ToListAsync();
                var tongTien = danhSachPhieuNhap.Sum(pn => pn.TongTien);

                ViewBag.TongTien = tongTien;
                return View(danhSachPhieuNhap);
            }

            [HttpGet("ExportPhieuNhapToExcel")]
            public async Task<IActionResult> ExportPhieuNhapToExcel(int? selectedMonth, int? selectedYearForMonth, int? selectedQuarter, int? selectedYearForQuarter, int? selectedYear)
            {
                var query = _context.vw_ThongKePhieuNhap.AsQueryable();

                // Lọc theo tháng và năm
                if (selectedMonth.HasValue && selectedYearForMonth.HasValue)
                {
                    query = query.Where(pn => pn.NgayNhap.Month == selectedMonth.Value && pn.NgayNhap.Year == selectedYearForMonth.Value);
                }
                // Lọc theo quý và năm
                else if (selectedQuarter.HasValue && selectedYearForQuarter.HasValue)
                {
                    int startMonth = (selectedQuarter.Value - 1) * 3 + 1;
                    int endMonth = startMonth + 2;
                    query = query.Where(pn => pn.NgayNhap.Year == selectedYearForQuarter.Value
                                               && pn.NgayNhap.Month >= startMonth
                                               && pn.NgayNhap.Month <= endMonth);
                }
                // Lọc theo năm
                else if (selectedYear.HasValue)
                {
                    query = query.Where(pn => pn.NgayNhap.Year == selectedYear.Value);
                }

                // Lấy danh sách phiếu nhập
                var danhSachPhieuNhap = await query.ToListAsync();

                // Tính tổng tiền
                var tongTien = danhSachPhieuNhap.Sum(pn => pn.TongTien);

                // Tạo file Excel
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("ThongKePhieuNhap");
                    worksheet.Cells[1, 1].Value = "Mã Phiếu Nhập";
                    worksheet.Cells[1, 2].Value = "Mã Nhân Viên";
                    worksheet.Cells[1, 3].Value = "Ngày Nhập";
                    worksheet.Cells[1, 4].Value = "Tổng Tiền";
                    worksheet.Cells[1, 5].Value = "Ghi Chú";
                    worksheet.Cells[1, 6].Value = "Nhà Cung Cấp";

                    for (int i = 0; i < danhSachPhieuNhap.Count; i++)
                    {
                        var phieuNhap = danhSachPhieuNhap[i];
                        worksheet.Cells[i + 2, 1].Value = phieuNhap.MaPhieuNhap;
                        worksheet.Cells[i + 2, 2].Value = phieuNhap.MaNhanVien;
                        worksheet.Cells[i + 2, 3].Value = phieuNhap.NgayNhap.ToString("dd/MM/yyyy");
                        worksheet.Cells[i + 2, 4].Value = phieuNhap.TongTien;
                        worksheet.Cells[i + 2, 5].Value = phieuNhap.GhiChu;
                        worksheet.Cells[i + 2, 6].Value = phieuNhap.NhaCungCap;
                    }

                    // Thêm hàng tổng cộng
                    worksheet.Cells[danhSachPhieuNhap.Count + 2, 3].Value = "Tổng cộng:";
                    worksheet.Cells[danhSachPhieuNhap.Count + 2, 4].Value = tongTien;

                    // Định dạng cho hàng tổng cộng
                    using (var range = worksheet.Cells[danhSachPhieuNhap.Count + 2, 3, danhSachPhieuNhap.Count + 2, 4])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    worksheet.Cells.AutoFitColumns();

                    // Trả về file Excel
                    var fileDownloadName = "ThongKePhieuNhap.xlsx";
                    var fileBytes = package.GetAsByteArray();
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileDownloadName);
                }
            }
        }
    }



