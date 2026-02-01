using Microsoft.AspNetCore.Mvc;

namespace ToanHocHay.WebApp.Models.DTOs
{
    public class StudentDashboardDto
    {
        public double AverageScore { get; set; }
        public int TotalAttempts { get; set; }
        public int CompletedChapters { get; set; }
        public List<ScoreChartItemDto> ChartData { get; set; } = new();
        public List<ChapterProgressDto> Chapters { get; set; } = new();
        public List<ExerciseAttemptDto> RecentAttempts { get; set; } = new();
    }

    public class ScoreChartItemDto
    {
        public string ChapterName { get; set; } = "";
        public double AvgScore { get; set; }
    }
    public class ChapterProgressDto
    {
        public int ChapterId { get; set; }
        public string ChapterName { get; set; }
        public int TotalLessons { get; set; }
        public int ProgressPercentage { get; set; } // Ví dụ: 45%
        public string Status { get; set; } // "Đã hoàn thành", "Đang học", "Chưa mở"
    }
}
