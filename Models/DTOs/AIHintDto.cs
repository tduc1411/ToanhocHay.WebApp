using System;

namespace ToanHocHay.WebApp.Models.DTOs
{
    public class AIHintRequestDto
    {
        public int AttemptId { get; set; }
        public int QuestionId { get; set; }
        public int HintLevel { get; set; }
        public string? StudentAnswer { get; set; }
    }

    public class AIHintDto
    {
        public int HintId { get; set; }
        public int AttemptId { get; set; }
        public int QuestionId { get; set; }
        public string HintText { get; set; } = string.Empty;
        public int HintLevel { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
