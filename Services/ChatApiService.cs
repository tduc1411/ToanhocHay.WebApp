using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ToanHocHay.WebApp.Services
{
    public class ChatApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _chatbotUrl;

        public ChatApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _chatbotUrl = configuration["AI:ChatbotApiUrl"] ?? "";
        }

        public async Task<JsonElement> SendMessageAsync(string userId, string text)
        {
            try
            {
                // Dùng class để kiểm soát JSON property names chính xác
                var payload = new ChatMessagePayload { UserId = userId, Text = text };
                var response = await _httpClient.PostAsJsonAsync($"{_chatbotUrl}/message", payload);

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<JsonElement>();
            }
            catch (Exception ex)
            {
                return JsonDocument.Parse("{\"success\": false, \"response\": {\"message\": \"Không thể kết nối tới server AI.\"}}").RootElement;
            }
        }

        public async Task<JsonElement> SendQuickReplyAsync(string userId, string reply)
        {
            try
            {
                var payload = new ChatReplyPayload { UserId = userId, Reply = reply };

                string baseUrl = _chatbotUrl.TrimEnd('/');
                string requestUrl = $"{baseUrl}/quick-reply";

                var response = await _httpClient.PostAsJsonAsync(requestUrl, payload);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<JsonElement>();
            }
            catch (Exception ex)
            {
                return JsonDocument.Parse("{\"response\": {\"message\": \"Lỗi kết nối AI. Kiểm tra Terminal Python ngay!\"}}").RootElement;
            }
        }

        public async Task<JsonElement> SendTriggerAsync(string userId, string trigger)
        {
            try
            {
                var payload = new ChatTriggerPayload { UserId = userId, Trigger = trigger };

                string baseUrl = _chatbotUrl.TrimEnd('/');
                string requestUrl = $"{baseUrl}/trigger";

                var response = await _httpClient.PostAsJsonAsync(requestUrl, payload);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                }

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                return result;
            }
            catch (Exception ex)
            {
                return JsonDocument.Parse("{\"response\": {\"message\": \"\"}}").RootElement;
            }
        }
    }

    // Payload classes với JsonPropertyName để kiểm soát JSON output
    public class ChatMessagePayload
    {
        [JsonPropertyName("UserId")]
        public string UserId { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class ChatReplyPayload
    {
        [JsonPropertyName("UserId")]
        public string UserId { get; set; }

        [JsonPropertyName("reply")]
        public string Reply { get; set; }
    }

    public class ChatTriggerPayload
    {
        [JsonPropertyName("UserId")]
        public string UserId { get; set; }

        [JsonPropertyName("trigger")]
        public string Trigger { get; set; }
    }
}