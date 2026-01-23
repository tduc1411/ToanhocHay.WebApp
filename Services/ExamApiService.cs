using System.Net.Http.Json;
using ToanHocHay.WebApp.Common.Constants;
using ToanHocHay.WebApp.Models.DTOs;

namespace ToanHocHay.WebApp.Services
{
    public class ExamApiService
    {
        private readonly HttpClient _httpClient;

        public ExamApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Lấy danh sách bài kiểm tra
        public async Task<List<ExerciseDto>> GetExercisesAsync()
        {
            try
            {
                var apiResponse = await _httpClient.GetFromJsonAsync<ApiResponse<List<ExerciseDto>>>($"{ApiConstant.apiBaseUrl}/api/Exercises");
                return apiResponse?.Data ?? new List<ExerciseDto>();
            }
            catch { return new List<ExerciseDto>(); }
        }

        // 1. Lấy chi tiết đề thi
        public async Task<ExerciseDetailDto?> GetExerciseById(int id)
        {
            try
            {
                var apiResponse = await _httpClient.GetFromJsonAsync<ApiResponse<ExerciseDetailDto>>($"{ApiConstant.apiBaseUrl}/api/exercises/{id}");
                return apiResponse?.Data;
            }
            catch { return null; }
        }

        // 2. Bắt đầu làm bài (Lấy AttemptId)
        public async Task<int> StartExercise(int exerciseId, int studentId)
        {
            var payload = new { ExerciseId = exerciseId, StudentId = studentId };
            var response = await _httpClient.PostAsJsonAsync($"{ApiConstant.apiBaseUrl}/api/ExerciseAttempts/start", payload);

            if (!response.IsSuccessStatusCode) return 0;

            var apiResult = await response.Content.ReadFromJsonAsync<ApiResponse<ExerciseAttemptResponseDto>>();
            return apiResult?.Data?.AttemptId ?? 0;
        }

        // 3. Nộp từng câu trả lời
        public async Task<bool> SubmitSingleAnswer(SubmitAnswerRequestDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiConstant.apiBaseUrl}/api/ExerciseAttempts/submit-answer", dto);
            return response.IsSuccessStatusCode;
        }

        // 4. Hoàn thành bài thi
        public async Task<bool> CompleteExercise(int attemptId)
        {
            var payload = new { AttemptId = attemptId };
            var response = await _httpClient.PostAsJsonAsync($"{ApiConstant.apiBaseUrl}/api/ExerciseAttempts/complete", payload);
            return response.IsSuccessStatusCode;
        }

        // 5. Lấy kết quả sau khi hoàn thành
        public async Task<ExerciseResultDto?> GetExerciseResult(int attemptId)
        {
            try
            {
                var apiResponse = await _httpClient.GetFromJsonAsync<ApiResponse<ExerciseResultDto>>($"{ApiConstant.apiBaseUrl}/api/ExerciseAttempts/{attemptId}/result");
                return apiResponse?.Data;
            }
            catch { return null; }
        }
    }
}