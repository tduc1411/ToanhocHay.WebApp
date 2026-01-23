namespace ToanHocHay.WebApp.Models.DTOs
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; }
        public string? Phone { get; set; }
        public string? SchoolName { get; set; }
        public int? GradeLevel { get; set; }
    }
}