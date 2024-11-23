using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class ChatBotController : Controller
    {
        private readonly QL_NhaThuocContext _context;

        public ChatBotController(QL_NhaThuocContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string searchString, string categoryFilter)
        {
            // Lấy danh sách câu hỏi thường gặp từ cơ sở dữ liệu
            var query = _context.Faqs.AsQueryable();

            // Lọc theo từ khóa tìm kiếm (Mã câu hỏi hoặc câu hỏi thường gặp)
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(f => f.MaCauHoi.ToString().Contains(searchString) || f.CauHoiThuongGap.Contains(searchString));
            }

            // Lọc theo danh mục câu hỏi
            if (!string.IsNullOrEmpty(categoryFilter))
            {
                query = query.Where(f => f.DanhMucCauHoi == categoryFilter);
            }

            // Lấy danh sách câu hỏi sau khi đã lọc
            var faqs = await query.Select(f => new Faq
            {
                MaCauHoi = f.MaCauHoi,
                CauHoiThuongGap = f.CauHoiThuongGap,
                CauTraLoiTuongUng = f.CauTraLoiTuongUng,
                DanhMucCauHoi = f.DanhMucCauHoi,
                NgayTaoCauHoi = f.NgayTaoCauHoi,
                NgayCapNhatCauHoi = f.NgayCapNhatCauHoi
            }).ToListAsync();

            // Trả về view với danh sách câu hỏi đã được lọc
            return View(faqs);
        }

        // Lấy danh sách câu hỏi thường gặp

        public IActionResult Index()
        {
            var faqList = _context.Faqs
                           .FromSqlRaw("EXEC sp_LayTatCaCauHoi")
                           .ToList();
            return View(faqList);
        }

        // Trang tạo mới câu hỏi thường gặp
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý tạo mới câu hỏi thường gặp
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Faq faq)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var parameters = new[]
                    {
                        faq.CauHoiThuongGap,
                        faq.CauTraLoiTuongUng,
                        faq.DanhMucCauHoi,
                        faq.NgayTaoCauHoi.ToString()
                    };
                    _context.Database.ExecuteSqlRaw("EXEC sp_ThemCauHoi @p0, @p1, @p2, @p3", parameters);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo câu hỏi: " + ex.Message);
                }
            }
            return View(faq);
        }

        // Trang chỉnh sửa câu hỏi
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var faq = _context.Faqs.FirstOrDefault(f => f.MaCauHoi == id);
            if (faq == null)
            {
                return NotFound();
            }
            return View(faq);
        }

        // Xử lý cập nhật câu hỏi
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Faq faq)
        {
            if (id != faq.MaCauHoi)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var parameters = new object[]
                    {
                        faq.MaCauHoi,
                        faq.CauHoiThuongGap,
                        faq.CauTraLoiTuongUng,
                        faq.DanhMucCauHoi,
                        faq.NgayCapNhatCauHoi.ToString()
                    };
                    _context.Database.ExecuteSqlInterpolated($"EXEC sp_CapNhatCauHoi {faq.MaCauHoi}, {faq.CauHoiThuongGap}, {faq.CauTraLoiTuongUng}, {faq.DanhMucCauHoi}, {faq.NgayCapNhatCauHoi}");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Faqs.Any(f => f.MaCauHoi == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(faq);
        }

        // Xóa câu hỏi
        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Json(new { success = false, message = "Không có câu hỏi nào được chọn để xóa." });
            }

            try
            {
                foreach (var id in ids)
                {
                    var parameter = new SqlParameter("@MaCauHoi", id);
                    _context.Database.ExecuteSqlRaw("EXEC sp_XoaCauHoi @MaCauHoi", parameter);
                }

                return Json(new { success = true, message = "Các câu hỏi đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

    }
}
