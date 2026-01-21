using Microsoft.AspNetCore.Mvc;
using ToanHocHay.WebApp.Models.DTOs;
using ToanHocHay.WebApp.Services;

namespace ToanHocHay.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthApiService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AuthApiService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // 1. Xử lý đường dẫn /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Mode = "login"; // Báo cho View hiển thị form đăng nhập
            ViewBag.Error = null;
            ViewBag.Email = null;
            return View(); // Tự động tìm file Views/Account/Login.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(string email, string password)
        {
            _logger.LogInformation("Login attempt with email: {Email}", email);
            var (data, error) = await _authService.Login(new LoginRequestDto
            {
                Email = email,
                Password = password
            });

            if (error != null)
            {
                _logger.LogWarning("Login failed for email: {Email}. Reason: {Reason}", email, error);

                ViewBag.Error = error;
                ViewBag.Mode = "login";
                ViewBag.Email = email;
                return View();
            }

            _logger.LogInformation("Login success. UserId: {UserId}, Email: {Email}, Role: {Role}", data!.UserId, data.Email, data.UserType);

            HttpContext.Session.SetString("JWT", data!.Token);
            HttpContext.Session.SetInt32("UserId", data!.UserId);
            HttpContext.Session.SetString("Email", data.Email);
            HttpContext.Session.SetString("UserType", data.UserType);

            return RedirectToAction("Index", "Home");
        }

        // 2. Xử lý đường dẫn /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Mode = "register"; // Báo cho View hiển thị form đăng ký
            return View("Login"); // Tái sử dụng file Login.cshtml
        }

        [HttpPost]
        public IActionResult Register(string fullName, string email, string password, string role)
        {
            // Logic tạo tài khoản sẽ viết ở đây
            return RedirectToAction("Index", "Home");
        }

        // 3. Xử lý đường dẫn /Account/Logout
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}