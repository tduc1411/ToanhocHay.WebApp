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
        public async Task<IActionResult> Index(int id = 1)
        {
            // Gọi Service với id linh hoạt
            var curriculum = await _courseApi.GetCurriculumDetailAsync(id);

            if (curriculum == null)
            {
                // Tránh lỗi null reference ở View bằng cách khởi tạo list trống
                return View(new CurriculumDto { Chapters = new List<ChapterDto>() });
            }

            return View(curriculum);
        }
        // 1. Trang danh sách bài học của một Chương
        // URL: /Course/Chapter/5
        public async Task<IActionResult> Chapter(int id)
        {
            // Bạn có thể dùng chung View Index nhưng lọc theo ChapterId
            // Hoặc lấy dữ liệu Curriculum rồi cuộn đến Chapter tương ứng
            var curriculum = await _courseApi.GetCurriculumDetailAsync(1); // Mặc định lớp 6 là ID 1
            ViewBag.SelectedChapterId = id;
            return View("Index", curriculum);
        }

        // 2. Trang danh sách bài học của một Chủ đề (Topic)
        // URL: /Course/Topic/10
        public async Task<IActionResult> Topic(int id)
        {
            // Tương tự, trả về view Index và focus vào Topic
            var curriculum = await _courseApi.GetCurriculumDetailAsync(1);
            ViewBag.SelectedTopicId = id;
            return View("Index", curriculum);
        }

        // Trang học bài chi tiết (Learning Player)
        // URL: /Course/Learning/id
        public async Task<IActionResult> Learning(int id)
        {
            var lesson = await _courseApi.GetLessonDetailAsync(id);
            if (lesson == null) return NotFound();

            // Lấy CurriculumId từ bài học (nếu DTO có) hoặc mặc định là 1
            int curriculumId = 1;
            var curriculum = await _courseApi.GetCurriculumDetailAsync(curriculumId);

            ViewBag.FullCurriculum = curriculum;
            ViewBag.CurrentTopicId = lesson.TopicId; // Để Highlight bài đang học ở menu bên phải

            return View("Lesson", lesson);
        }
    }
}