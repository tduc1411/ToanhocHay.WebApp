using Microsoft.AspNetCore.Mvc;
using ToanHocHay.WebApp.Services;
using System.Security.Claims;
using System.Text.Json.Serialization; // Cần thiết để mapping tên trường

namespace ToanHocHay.WebApp.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatApiService _chatService;

        public ChatController(ChatApiService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] ChatMessageRequest request)
        {
            // 1. Lấy UserId từ hệ thống (Identity/Session)
            string userId = GetUserId();
            
            // 2. Gửi sang Service. 
            // Đảm bảo trong _chatService.SendMessageAsync, dữ liệu được gửi đi với key là "UserId"
            var result = await _chatService.SendMessageAsync(userId, request.Text);
            
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> QuickReply([FromBody] QuickReplyRequest request)
        {
            string userId = GetUserId();
            var result = await _chatService.SendQuickReplyAsync(userId, request.Reply);
            return Json(result);
        }

        private string GetUserId()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                // Lấy ID từ Database người dùng đã đăng nhập
                return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "guest";
            }
            // Nếu là khách, dùng ID phiên làm việc tạm thời
            return HttpContext.Session.Id ?? "anonymous";
        }
    }

    // --- Sửa lại các Class Request để khớp chuẩn Database UserId ---

    public class ChatMessageRequest 
    { 
        [JsonPropertyName("UserId")] // Khớp với Database
        public string? UserId { get; set; } 

        [JsonPropertyName("text")]   // Khớp với Python API (thường dùng chữ thường)
        public string Text { get; set; } = string.Empty;
    }

    public class QuickReplyRequest 
    { 
        [JsonPropertyName("UserId")] // Khớp với Database
        public string? UserId { get; set; }

        [JsonPropertyName("reply")]  // Khớp với Python API
        public string Reply { get; set; } = string.Empty;
    }
}