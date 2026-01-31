using Microsoft.AspNetCore.Mvc;
using ToanHocHay.WebApp.Services;
using System.Security.Claims;
using System.Text.Json.Serialization; // Cần thiết để mapping tên trường

namespace ToanHocHay.WebApp.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatApiService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(ChatApiService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] ChatMessageRequest request)
        {
            try
            {
                string userId = GetUserId();
                var result = await _chatService.SendMessageAsync(userId, request.Text);
                return Json(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> QuickReply([FromBody] QuickReplyRequest request)
        {
            try
            {
                string userId = GetUserId();
                var result = await _chatService.SendQuickReplyAsync(userId, request.Reply);
                return Json(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Trigger([FromBody] TriggerRequest request)
        {
            try
            {
                string userId = GetUserId();
                var result = await _chatService.SendTriggerAsync(userId, request.TriggerType);
                return Json(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GetUserId()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "guest";
                return userId;
            }
            var sessionId = HttpContext.Session.Id ?? "anonymous";
            return sessionId;
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

    public class TriggerRequest
    {
        [JsonPropertyName("trigger")]
        public string TriggerType { get; set; } = string.Empty;
    }
}