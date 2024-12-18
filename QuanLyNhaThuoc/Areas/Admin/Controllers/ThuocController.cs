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
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10;

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentPage = page;

            var thuocQuery = _context.Thuocs.Include(t => t.HinhAnhs).AsQueryable();

            // điều kiện lọc
            if (!string.IsNullOrEmpty(searchString))
            {
                thuocQuery = thuocQuery.Where(t => t.TenThuoc.Contains(searchString));
            }

            // tổng số trang
            int totalItems = await thuocQuery.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // phân trang
            var thuocList = await thuocQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

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
                //procedure thêm thuốc và tồn kho
                var parameters = new[]
                {
            new SqlParameter("@TenThuoc", thuoc.TenThuoc),
            new SqlParameter("@HanSuDung", SqlDbType.Date) { Value = thuoc.HanSuDung ?? (object)DBNull.Value },
            new SqlParameter("@DonGia", thuoc.DonGia),
            new SqlParameter("@SoLuongTon", thuoc.SoLuongTon),
            new SqlParameter("@MaLoaiSanPham", thuoc.MaLoaiSanPham),
            new SqlParameter("@DonVi", thuoc.DonVi),
            new SqlParameter("@SoLuongCanhBao", 10),
            new SqlParameter("@SoLuongToiDa", 10000)
        };

                // thêm thuốc lấy mã thuốc mới
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_ThemThuocVaTonKho @TenThuoc, @HanSuDung, @DonGia, @SoLuongTon, @MaLoaiSanPham, @DonVi, @SoLuongCanhBao, @SoLuongToiDa", parameters);

                thuoc.MaThuoc = await _context.Thuocs
                    .Where(t => t.TenThuoc == thuoc.TenThuoc && t.HanSuDung == thuoc.HanSuDung)
                    .Select(t => t.MaThuoc)
                    .FirstOrDefaultAsync();

                // thêm hình
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
            // lay thuoc va danh sach hinh
            var thuoc = await _context.Thuocs
                                      .Include(t => t.HinhAnhs) 
                                      .FirstOrDefaultAsync(t => t.MaThuoc == id);

            if (thuoc == null)
            {
                return NotFound();
            }

            // tao list loai san pham
            ViewData["MaLoaiSanPham"] = new SelectList(_context.LoaiSanPhams, "MaLoaiSanPham", "TenLoai", thuoc.MaLoaiSanPham);
            return View(thuoc); 
        }


        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Thuoc thuoc, IFormFileCollection ImageFiles)
        {
            if (!ModelState.IsValid)
            {
                //du lieu loi tra ve form 
                ViewData["MaLoaiSanPham"] = new SelectList(_context.LoaiSanPhams, "MaLoaiSanPham", "TenLoai", thuoc.MaLoaiSanPham);
                return View(thuoc);
            }

            //lay thuoc hien tai
            var existingThuoc = await _context.Thuocs.Include(t => t.HinhAnhs).FirstOrDefaultAsync(t => t.MaThuoc == id);
            if (existingThuoc == null)
            {
                return NotFound();
            }

            //cap nhat thuoc
            existingThuoc.TenThuoc = thuoc.TenThuoc;
            existingThuoc.HanSuDung = thuoc.HanSuDung;
            existingThuoc.DonVi = thuoc.DonVi;
            existingThuoc.DonGia = thuoc.DonGia;
            existingThuoc.SoLuongTon = thuoc.SoLuongTon;
            existingThuoc.MaLoaiSanPham = thuoc.MaLoaiSanPham;

            // xu ly anh moi
            if (ImageFiles != null && ImageFiles.Count > 0)
            {
                // xoa anh cu va trong folder
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

                // Luu anh
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
                //procedure xóa thuốc
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_XoaThuocTonKho @p0", id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", $"Không thể xóa thuốc này: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
