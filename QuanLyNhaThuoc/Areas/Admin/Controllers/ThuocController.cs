using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class ThuocController : Controller
    {
        private readonly QL_NhaThuocContext _context;

        public ThuocController(QL_NhaThuocContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var thuocList = await _context.Thuocs.Include(t => t.HinhAnhs).ToListAsync();
            return View(thuocList);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["MaLoaiSanPham"] = new SelectList(_context.LoaiSanPhams, "MaLoaiSanPham", "TenLoai");
            return View();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(Thuoc thuoc, IFormFileCollection ImageFiles)
        {
            if (ModelState.IsValid)
            {
                // Gọi stored procedure để thêm thuốc và tồn kho
                var parameters = new[]
                {
            new SqlParameter("@TenThuoc", thuoc.TenThuoc),
            new SqlParameter("@HanSuDung", SqlDbType.Date) { Value = thuoc.HanSuDung ?? (object)DBNull.Value },
            new SqlParameter("@DonGia", thuoc.DonGia),
            new SqlParameter("@SoLuongTon", thuoc.SoLuongTon),
            new SqlParameter("@MaLoaiSanPham", thuoc.MaLoaiSanPham),
            new SqlParameter("@DonVi", thuoc.DonVi),
            new SqlParameter("@SoLuongCanhBao", 10),
            new SqlParameter("@SoLuongToiDa", 100)
        };

                // Thêm thuốc và lấy mã thuốc mới
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_ThemThuocVaTonKho @TenThuoc, @HanSuDung, @DonGia, @SoLuongTon, @MaLoaiSanPham, @DonVi, @SoLuongCanhBao, @SoLuongToiDa", parameters);

                // Lấy mã thuốc mới nhất
                thuoc.MaThuoc = await _context.Thuocs
                    .Where(t => t.TenThuoc == thuoc.TenThuoc && t.HanSuDung == thuoc.HanSuDung)
                    .Select(t => t.MaThuoc)
                    .FirstOrDefaultAsync();

                // Thêm hình ảnh nếu có
                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    foreach (var imageFile in ImageFiles)
                    {
                        if (imageFile.Length > 0)
                        {
                            var fileName = Path.GetFileName(imageFile.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }

                            var hinhAnh = new HinhAnh
                            {
                                MaThuoc = thuoc.MaThuoc, 
                                UrlAnh = "/images/" + fileName
                            };
                            _context.HinhAnhs.Add(hinhAnh);
                        }
                    }

                    await _context.SaveChangesAsync(); 
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["MaLoaiSanPham"] = new SelectList(_context.LoaiSanPhams, "MaLoaiSanPham", "TenLoai", thuoc.MaLoaiSanPham);
            return View(thuoc);
        }


        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var thuoc = await _context.Thuocs
                                      .Include(t => t.HinhAnhs) 
                                      .FirstOrDefaultAsync(t => t.MaThuoc == id);

            if (thuoc == null)
            {
                return NotFound();
            }

            ViewData["MaLoaiSanPham"] = new SelectList(_context.LoaiSanPhams, "MaLoaiSanPham", "TenLoai", thuoc.MaLoaiSanPham);
            return View(thuoc); 
        }


        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Thuoc thuoc, IFormFileCollection ImageFiles)
        {
            if (!ModelState.IsValid)
            {
                ViewData["MaLoaiSanPham"] = new SelectList(_context.LoaiSanPhams, "MaLoaiSanPham", "TenLoai", thuoc.MaLoaiSanPham);
                return View(thuoc);
            }

            // Cập nhật thông tin thuốc
            var existingThuoc = await _context.Thuocs.Include(t => t.HinhAnhs).FirstOrDefaultAsync(t => t.MaThuoc == id);
            if (existingThuoc == null)
            {
                return NotFound();
            }

            existingThuoc.TenThuoc = thuoc.TenThuoc;
            existingThuoc.HanSuDung = thuoc.HanSuDung;
            existingThuoc.DonVi = thuoc.DonVi;
            existingThuoc.DonGia = thuoc.DonGia;
            existingThuoc.SoLuongTon = thuoc.SoLuongTon;
            existingThuoc.MaLoaiSanPham = thuoc.MaLoaiSanPham;

            // Nếu có ảnh mới, thay thế ảnh cũ
            if (ImageFiles != null && ImageFiles.Count > 0)
            {
                // Xóa ảnh cũ
                if (existingThuoc.HinhAnhs != null)
                {
                    foreach (var hinhAnh in existingThuoc.HinhAnhs)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", hinhAnh.UrlAnh.TrimStart('/'));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    _context.HinhAnhs.RemoveRange(existingThuoc.HinhAnhs);
                }

                // Lưu ảnh mới
                foreach (var imageFile in ImageFiles)
                {
                    if (imageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        var hinhAnh = new HinhAnh
                        {
                            MaThuoc = existingThuoc.MaThuoc,
                            UrlAnh = "/images/" + fileName
                        };
                        _context.HinhAnhs.Add(hinhAnh);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ThuocExists(int id)
        {
            return _context.Thuocs.Any(e => e.MaThuoc == id);
        }


        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Gọi stored procedure xóa thuốc và tồn kho liên quan
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_XoaThuocTonKho @p0", id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // Bắt lỗi và trả về thông báo lỗi chi tiết
                ModelState.AddModelError("", $"Không thể xóa thuốc này: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
