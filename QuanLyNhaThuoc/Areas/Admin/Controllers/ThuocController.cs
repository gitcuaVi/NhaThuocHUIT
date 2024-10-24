using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
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
            var thuocList = await _context.Thuocs
                                          .Include(t => t.HinhAnhs) 
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
 
                _context.Thuocs.Add(thuoc);
                await _context.SaveChangesAsync();  

                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    foreach (var imageFile in ImageFiles)
                    {
                        if (imageFile != null && imageFile.Length > 0)
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
            if (id != thuoc.MaThuoc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string urlAnh = null;

                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    foreach (var imageFile in ImageFiles)
                    {
                        if (imageFile != null && imageFile.Length > 0)
                        {
                            var fileName = Path.GetFileName(imageFile.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }

                            urlAnh = "/images/" + fileName;
                            break;
                        }
                    }
                }

                await _context.Database.ExecuteSqlRawAsync(
                    "sp_CapNhatThuoc @p0, @p1, @p2, @p3, @p4, @p5",
                    thuoc.MaThuoc, thuoc.TenThuoc, thuoc.HanSuDung, thuoc.DonGia, thuoc.SoLuongTon, thuoc.MaLoaiSanPham, urlAnh);

                return RedirectToAction(nameof(Index));
            }

            ViewData["MaLoaiSanPham"] = new SelectList(_context.LoaiSanPhams, "MaLoaiSanPham", "TenLoai", thuoc.MaLoaiSanPham);
            return View(thuoc);
        }


        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var thuoc = await _context.Thuocs
                                      .Include(t => t.HinhAnhs)
                                      .FirstOrDefaultAsync(t => t.MaThuoc == id);

            if (thuoc == null)
            {
                return NotFound();
            }

            // Xóa file hình ảnh khỏi thư mục wwwroot
            if (thuoc.HinhAnhs != null && thuoc.HinhAnhs.Any())
            {
                foreach (var hinhAnh in thuoc.HinhAnhs)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", hinhAnh.UrlAnh.TrimStart('/'));

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath); 
                    }
                }
            }

            await _context.Database.ExecuteSqlRawAsync("sp_XoaThuoc @p0", id);

            return RedirectToAction(nameof(Index));
        }



    }
}
