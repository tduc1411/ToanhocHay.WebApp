using System.Net.Http.Json;
using System.Text.Json;
using ToanHocHay.WebApp.Common.Constants;
using ToanHocHay.WebApp.Models.DTOs;

namespace ToanHocHay.WebApp.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public AuthApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // 1. Đăng nhập
        public async Task<(LoginResponseDto? data, string? error)> Login(LoginRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiConstant.apiBaseUrl}/api/auth/login", request);
            var resString = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(resString, _jsonOptions);

            if (!response.IsSuccessStatusCode)
            {
                return (null, apiResponse?.Message ?? "Đăng nhập thất bại");
            }
            return (apiResponse!.Data, null);
        }

        // 2. Đăng ký
        public async Task<(bool success, string? error)> Register(RegisterRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiConstant.apiBaseUrl}/api/auth/register", request);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();

            if (!response.IsSuccessStatusCode || apiResponse == null || !apiResponse.Success)
            {
                return (false, apiResponse?.Message ?? "Đăng ký thất bại");
            }
            return (true, null);
        }

        // 3. Lấy thông tin Profile mới nhất (Sửa lỗi thiếu định nghĩa)
        public async Task<UserDto?> GetProfileAsync(int userId)
        {
            try
            {
                var apiResponse = await _httpClient.GetFromJsonAsync<ApiResponse<UserDto>>($"{ApiConstant.apiBaseUrl}/api/auth/profile/{userId}");
                return apiResponse?.Data;
            }
            catch { return null; }
        }

        // 4. Cập nhật thông tin cá nhân (Sửa lỗi thiếu định nghĩa)
        public async Task<ApiResponse<bool>> UpdateProfileAsync(int userId, UpdateProfileDto request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiConstant.apiBaseUrl}/api/auth/update-profile/{userId}", request);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            return result ?? ApiResponse<bool>.ErrorResponse("Lỗi kết nối API");
        }

        // 5. Đổi mật khẩu (Sửa lỗi thiếu định nghĩa)
        public async Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiConstant.apiBaseUrl}/api/auth/change-password/{userId}", request);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            return result ?? ApiResponse<bool>.ErrorResponse("Lỗi kết nối API");
        }
    }
}