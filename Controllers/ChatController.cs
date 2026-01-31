using Microsoft.AspNetCore.Mvc;
using ToanHocHay.WebApp.Services;
using System.Security.Claims;

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
            string userId = GetUserId();
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
                return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "guest";
            }
            // Nếu chưa đăng nhập, dùng Session ID để phân biệt các khách truy cập
            return HttpContext.Session.Id ?? "anonymous";
        }
    }

    public class ChatMessageRequest { public string Text { get; set; } }
    public class QuickReplyRequest { public string Reply { get; set; } }
}