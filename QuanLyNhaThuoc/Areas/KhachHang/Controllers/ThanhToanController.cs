using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaThuoc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang/[controller]")]
    public class ThanhToanController : Controller
    {
        private readonly QL_NhaThuocContext _context;

        public ThanhToanController(QL_NhaThuocContext context)
        {
            _context = context;
        }
       



    }
}
