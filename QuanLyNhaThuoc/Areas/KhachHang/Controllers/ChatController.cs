using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;
using System.Data;
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

                // Gọi stored procedure để tìm câu trả lời
                var responseMessage = await GetFaqAnswerFromProcedure(userQuestion);

                return Json(new { success = true, responseMessage });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi xử lý yêu cầu: " + ex.Message);
                return Json(new { success = false, responseMessage = "Đã xảy ra lỗi khi xử lý yêu cầu." });
            }
        }

        private async Task<string> GetFaqAnswerFromProcedure(string userQuestion)
        {
            using (var command = db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "dbo.GetFaqAnswer";
                command.CommandType = CommandType.StoredProcedure;

                // Thêm tham số đầu vào cho stored procedure
                var param = new SqlParameter("@UserQuestion", SqlDbType.NVarChar) { Value = userQuestion };
                command.Parameters.Add(param);

                db.Database.OpenConnection();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return reader["ResponseMessage"].ToString();
                    }
                }

                return "Xin lỗi, chúng tôi không thể tìm thấy câu trả lời phù hợp cho câu hỏi của bạn.";
            }
        }
    }
}

