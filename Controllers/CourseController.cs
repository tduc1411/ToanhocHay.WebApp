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
            // 1. Lấy chi tiết bài học hiện tại (để hiện nội dung chính)
            var lesson = await _courseApi.GetLessonDetailAsync(id);
            if (lesson == null) return NotFound();

            // 2. Lấy toàn bộ cấu trúc chương trình (để hiện danh sách chương/bài bên phải)
            // Giả sử bài học thuộc CurriculumId = 1, bạn có thể lấy động từ lesson nếu có field này
            var curriculum = await _courseApi.GetCurriculumDetailAsync(1);
            ViewBag.FullCurriculum = curriculum;

            return View("Lesson", lesson);
        }
    }
}