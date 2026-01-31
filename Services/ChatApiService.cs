using System.Text;
using System.Text.Json;

namespace ToanHocHay.WebApp.Services
{
    public class ChatApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _chatbotUrl;

        public ChatApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Lấy URL từ appsettings.json
            _chatbotUrl = configuration["AI:ChatbotApiUrl"];
        }

        public async Task<JsonElement> SendMessageAsync(string userId, string text)
        {
            var payload = new { user_id = userId, text = text };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_chatbotUrl}/message", content);
            return await response.Content.ReadFromJsonAsync<JsonElement>();
        }

        public async Task<JsonElement> SendQuickReplyAsync(string userId, string reply)
        {
            var payload = new { user_id = userId, reply = reply };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_chatbotUrl}/quick-reply", content);
            return await response.Content.ReadFromJsonAsync<JsonElement>();
        }
    }
}