using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models; 
using System.Linq;

namespace QuanLyNhaThuoc.Areas.Admin.Controllers
{
    [Authorize(Roles = "NhanVien")]
   [Area("Admin")]
    [Route("Admin/[controller]")]
    public class UserController : Controller
    {
        private readonly QL_NhaThuocContext db;
        private readonly ILogger<NguoiDungController> _logger;

        public UserController(QL_NhaThuocContext context, ILogger<NguoiDungController> logger)
        {
            db = context;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Profile()
        {
            var maNguoiDungClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (maNguoiDungClaim != null)
            {
                int maNguoiDung = int.Parse(maNguoiDungClaim.Value);
                var userInfo = db.ThongTinCaNhanView.FirstOrDefault(u => u.MaNguoiDung == maNguoiDung);

                if (userInfo != null)
                {
                    return View(userInfo);  
                }
                else
                {
                    return NotFound("Không tìm thấy thông tin người dùng.");
                }
            }
            else
            {
                return Unauthorized("Bạn chưa đăng nhập.");
            }
        }

    }
}
