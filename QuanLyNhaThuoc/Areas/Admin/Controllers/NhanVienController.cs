using Microsoft.AspNetCore.Mvc;
using QuanLyNhaThuoc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class NhanVienController : Controller
    {
        private readonly QL_NhaThuocContext db;
        private readonly ILogger<NhanVienController> _logger;

        public NhanVienController(QL_NhaThuocContext context, ILogger<NhanVienController> logger)
        {
            db = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(string searchString, string sexFilter, string statusFilter, int page = 1, int pageSize = 6)
        {
            // lọc gt
            ViewBag.SexList = new List<string> { "Nam", "Nữ" }; 
            var emp = from u in db.NhanViens select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                emp = emp.Where(u => u.Ho.Contains(searchString) || u.Ten.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(sexFilter))
            {
                emp = emp.Where(u => u.GioiTinh == sexFilter);
            }
            if (!string.IsNullOrEmpty(statusFilter))
            {
                emp = emp.Where(u => u.TrangThai == statusFilter);
            }

            // phan trang
            int totalItems = emp.Count();
            emp = emp.Skip((page - 1) * pageSize).Take(pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentFilter = searchString;
            ViewBag.SexFilter = sexFilter;
            ViewBag.StatusFilter = statusFilter;

            return View(emp.ToList());
        }
        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            // Lấy danh sách ca làm việc
            var caLamViecs = db.CaLamViecs.Select(c => new
            {
                c.MaCaLam,
                DisplayText = $"{c.MaCaLam} - {c.ThoiGianBatDau} đến {c.ThoiGianKetThuc}"
            }).ToList();

            // Lấy danh sách người dùng
            var nguoiDungs = db.NguoiDungs.Select(n => new
            {
                n.MaNguoiDung,
                DisplayText = $"{n.TenNguoiDung} ({n.Email})"
            }).ToList();

            // Truyền dữ liệu qua ViewBag
            ViewBag.CaLamViecs = new SelectList(caLamViecs, "MaCaLam", "DisplayText");
            ViewBag.NguoiDungs = new SelectList(nguoiDungs, "MaNguoiDung", "DisplayText");

            return View();
        }


        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string Ho, string Ten, DateTime NgaySinh, string GioiTinh, string DiaChi, string ChucVu, DateTime NgayTuyenDung, string TrangThai, int MaNguoiDung, int MaCaLamViec, decimal? LuongCoBan1Ca, decimal? LuongTangCa1Gio)
        {
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@Ho", Ho),
            new SqlParameter("@Ten", Ten),
            new SqlParameter("@NgaySinh", NgaySinh),
            new SqlParameter("@GioiTinh", GioiTinh),
            new SqlParameter("@DiaChi", DiaChi),
            new SqlParameter("@ChucVu", ChucVu),
            new SqlParameter("@NgayTuyenDung", NgayTuyenDung),
            new SqlParameter("@MaNguoiDung", (object)MaNguoiDung?? DBNull.Value),
            new SqlParameter("@MaCaLamViec",(object) MaCaLamViec?? DBNull.Value),
            new SqlParameter("@LuongCoBan1Ca", (object)LuongCoBan1Ca ?? DBNull.Value),
            new SqlParameter("@LuongTangCa1Gio", (object)LuongTangCa1Gio ?? DBNull.Value),
        };
                //gọi pro
                db.Database.ExecuteSqlRaw("EXEC sp_ThemNhanVienMoi @Ho, @Ten, @NgaySinh, @GioiTinh, @DiaChi, @ChucVu, @NgayTuyenDung, @MaNguoiDung, @MaCaLamViec, @LuongCoBan1Ca, @LuongTangCa1Gio", parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm nhân viên.");
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi thêm nhân viên.";
                return View(); 
            }
        }

        [Route("Edit/{id}")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            // Truyền mã nhân viên sang view
            return View(nhanVien);
        }

        [Route("Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string Ho, string Ten, DateTime? NgaySinh, string GioiTinh, string DiaChi, string ChucVu, DateTime? NgayTuyenDung, string TrangThai, int? MaNguoiDung, int? MaCaLamViec, decimal? LuongCoBan1Ca, decimal? LuongTangCa1Gio)
        {
            try
            {
                var nhanVien = db.NhanViens.Find(id);  // Tìm nhân viên trước khi cập nhật
                if (nhanVien == null)
                {
                    return NotFound();
                }

                var parameters = new[]
                {
            new SqlParameter("@MaNhanVien", id),
            new SqlParameter("@Ho", (object)Ho ?? DBNull.Value),
            new SqlParameter("@Ten", (object)Ten ?? DBNull.Value),
            new SqlParameter("@NgaySinh", (object)NgaySinh ?? DBNull.Value),
            new SqlParameter("@GioiTinh", (object)GioiTinh ?? DBNull.Value),
            new SqlParameter("@DiaChi", (object)DiaChi ?? DBNull.Value),
            new SqlParameter("@ChucVu", (object)ChucVu ?? DBNull.Value),
            new SqlParameter("@NgayTuyenDung", (object)NgayTuyenDung ?? DBNull.Value),
            new SqlParameter("@TrangThai", (object)TrangThai ?? DBNull.Value),
            new SqlParameter("@MaNguoiDung", (object)MaNguoiDung ?? DBNull.Value),
            new SqlParameter("@MaCaLamViec", (object)MaCaLamViec ?? DBNull.Value),
            new SqlParameter("@LuongCoBan1Ca", (object)LuongCoBan1Ca ?? DBNull.Value),
            new SqlParameter("@LuongTangCa1Gio", (object)LuongTangCa1Gio ?? DBNull.Value)
        };

                db.Database.ExecuteSqlRaw("EXEC sp_CapNhatThongTinNhanVien @MaNhanVien, @Ho, @Ten, @NgaySinh, @GioiTinh, @DiaChi, @ChucVu, @NgayTuyenDung, @TrangThai, @MaNguoiDung, @MaCaLamViec, @LuongCoBan1Ca, @LuongTangCa1Gio", parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật nhân viên.");
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi cập nhật nhân viên.";
                return View();
            }
        }
        [Route("Delete")]
        [HttpPost]
        public IActionResult Delete(int maNhanVien)
        {
            try
            {
                // Thiết lập tham số 
                var parameters = new SqlParameter("@MaNhanVien", maNhanVien);

                var result = new SqlParameter("@ReturnVal", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                };

                db.Database.ExecuteSqlRaw("EXEC @ReturnVal = sp_XoaNhanVien @MaNhanVien", parameters, result);

                if ((int)result.Value == 2)
                {
                    return Json(new { success = false, message = "Nhân viên không ở trạng thái 'Đã nghỉ', không thể xóa." });
                }

                return Json(new { success = true, message = "Xóa nhân viên thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa nhân viên.");
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa nhân viên." });
            }
        }
        [Route("TinhLuong")]
        [HttpGet]
        public IActionResult TinhLuong(int maNhanVien)
        {
            // Lấy tt nhân viên theo mã
            var nhanVien = db.NhanViens.Find(maNhanVien);
            if (nhanVien == null)
            {
                return NotFound();
            }

            // Truyền mã nhân viên sang view
            ViewBag.MaNhanVien = maNhanVien;
            return View();
        }

        [Route("TinhLuong")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TinhLuong(int maNhanVien, DateTime ngayTraLuong, decimal khauTru, int soGioTangCa = 0, decimal? luongThuong = null, string ghiChu = "")
        {
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@MaNhanVien", maNhanVien),
            new SqlParameter("@KhauTru", khauTru),
            new SqlParameter("@NgayTraLuong", ngayTraLuong),
            new SqlParameter("@SoGioTangCa", soGioTangCa),
            new SqlParameter("@LuongThuong", (object)luongThuong ?? DBNull.Value),
            new SqlParameter("@GhiChu", (object)ghiChu ?? DBNull.Value)
        };

                // Gọi pro
                db.Database.ExecuteSqlRaw("EXEC TinhLuong @MaNhanVien, @KhauTru, @NgayTraLuong, @SoGioTangCa, @LuongThuong, @GhiChu", parameters);

                return RedirectToAction("Index", new { successMessage = "Tính lương thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính lương.");
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tính lương.";
                return View();
            }
        }
        [Route("BangLuong")]
        [HttpGet]
        public IActionResult XemBangLuong(int page = 1, int pageSize = 6, string filterMonth = "")
        {
            var bangLuong = from l in db.Luongs
                            join n in db.NhanViens on l.MaNhanVien equals n.MaNhanVien
                            select new
                            {
                                l.MaNhanVien,
                                n.Ho,
                                n.Ten,
                                l.LuongThang,
                                l.SoCaLamViec,
                                l.LuongThucNhan,
                                l.KhauTru,
                                l.LuongThuong,
                                l.SoGioTangCa,
                                l.NgayTraLuong,
                                l.GhiChu
                            };

            // Lọc theo tháng
            if (!string.IsNullOrEmpty(filterMonth))
            {
                DateTime selectedMonth = DateTime.ParseExact(filterMonth, "yyyy-MM", null);
                bangLuong = bangLuong.Where(l => l.LuongThang.Year == selectedMonth.Year && l.LuongThang.Month == selectedMonth.Month);
            }

            // Phan trang
            int totalItems = bangLuong.Count();
            bangLuong = bangLuong.Skip((page - 1) * pageSize).Take(pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.FilterMonth = filterMonth;

            return View(bangLuong.ToList());
        }


    }
}

