using Microsoft.AspNetCore.Mvc;
using ToanHocHay.WebApp.Services;
using ToanHocHay.WebApp.Models.DTOs;

namespace ToanHocHay.WebApp.Controllers
{
    public class CourseController : Controller
    {
        private readonly CourseApiService _courseApi;

        public CourseController(CourseApiService courseApi)
        {
            _courseApi = courseApi;
        }

        // Trang hiển thị lộ trình học tập chi tiết (Chapters -> Topics -> Lessons)
        // URL: /Course/Index
        public async Task<IActionResult> Index()
        {
            var curriculum = await _courseApi.GetCurriculumDetailAsync(1);

            // THÊM DÒNG NÀY ĐỂ KIỂM TRA TRONG CỬA SỔ OUTPUT
            if (curriculum != null)
            {
                System.Diagnostics.Debug.WriteLine($"CHECK: Đã nhận được {curriculum.Chapters?.Count ?? 0} chương từ Service");
            }

            if (curriculum == null)
            {
                return View(new CurriculumDto { Chapters = new List<ChapterDto>() });
            }

            return View(curriculum);
        }

        // Trang học bài chi tiết (Learning Player)
        // URL: /Course/Learning/id
        public async Task<IActionResult> Learning(int id)
        {
            var lesson = await _courseApi.GetLessonDetailAsync(id);
            if (lesson == null) return NotFound();

            // Lấy danh sách bài học liên quan trong cùng chủ đề để hiện ở Sidebar
            ViewBag.RelatedLessons = await _courseApi.GetLessonsByTopicAsync(lesson.TopicId);

            return View(lesson);
        }
    }
}