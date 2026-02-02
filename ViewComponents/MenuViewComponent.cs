using Microsoft.AspNetCore.Mvc;
using ToanHocHay.WebApp.Services;

namespace ToanHocHay.WebApp.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly CourseApiService _courseApiService;

        public MenuViewComponent(CourseApiService courseApiService)
        {
            _courseApiService = courseApiService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = await _courseApiService.GetFullMenuTreeAsync();
            return View(data);
        }
    }
}