using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyNhaThuoc.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Route("KhachHang/[controller]")]
    public class ChatController : Controller
    {
        private readonly QL_NhaThuocContext db;

        public ChatController(QL_NhaThuocContext context)
        {
            db = context;
        }

        [HttpPost]
        [Route("GetFaqAnswer")]
        public async Task<JsonResult> GetFaqAnswer([FromBody] string userQuestion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userQuestion))
                {
                    return Json(new { success = false, responseMessage = "Vui lòng nhập câu hỏi." });
                }

                var normalizedUserQuestion = userQuestion.Trim().ToLower();
                var responseMessage = "Xin lỗi, chúng tôi không thể tìm thấy câu trả lời chính xác cho câu hỏi của bạn. Đây là một số câu trả lời gần đúng:";

                // Tìm câu hỏi hoàn toàn khớp
                var exactMatch = await db.Faqs
                                         .Where(f => f.CauHoiThuongGap.ToLower() == normalizedUserQuestion)
                                         .Select(f => f.CauTraLoiTuongUng)
                                         .FirstOrDefaultAsync();

                // Nếu tìm thấy câu hỏi hoàn toàn khớp, trả về câu trả lời tương ứng
                if (exactMatch != null)
                {
                    return Json(new { success = true, responseMessage = exactMatch });
                }

                // Nếu không có kết quả hoàn toàn khớp, tìm câu hỏi gần đúng
                var similarMatches = await db.Faqs
                                             .Where(f => f.CauHoiThuongGap.ToLower().Contains(normalizedUserQuestion))
                                             .Select(f => f.CauTraLoiTuongUng)
                                             .Take(3) // Lấy tối đa 3 câu trả lời gần đúng
                                             .ToListAsync();

                // Nếu có các câu hỏi gần đúng, trả về các câu trả lời này
                if (similarMatches.Any())
                {
                    responseMessage += string.Join("\n- ", similarMatches.Select(answer => $"- {answer}"));
                    return Json(new { success = true, responseMessage });
                }

                // Nếu không tìm thấy kết quả nào, trả về câu trả lời mặc định
                return Json(new { success = true, responseMessage = "Xin lỗi, chúng tôi không thể tìm thấy câu trả lời phù hợp cho câu hỏi của bạn." });
            }
            catch (Exception ex)
            {
                // Ghi log chi tiết lỗi để kiểm tra
                Console.WriteLine("Lỗi khi xử lý yêu cầu: " + ex.Message);
                return Json(new { success = false, responseMessage = "Đã xảy ra lỗi khi xử lý yêu cầu." });
            }
        }


    }


}

