using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToanHocHay.WebApp.Common;
using ToanHocHay.WebApp.Common.Constants;
using ToanHocHay.WebApp.Models.DTOs;
using ToanHocHay.WebApp.Services;

namespace ToanHocHay.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthApiService _authService;
        private readonly ILogger<AccountController> _logger;
        private readonly HttpClient _httpClient;

        public AccountController(AuthApiService authService, ILogger<AccountController> logger, IHttpClientFactory httpClientFactory)
        {
            _authService = authService;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
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

            /*_logger.LogInformation("UserType enum value: {Value}, Name: {Name}", 
                (int)data.UserType,
                data.UserType.ToString()
                );*/


            if (error != null)
            {
                _logger.LogWarning("Login failed for email: {Email}. Reason: {Reason}", email, error);

                ViewBag.Error = error;
                ViewBag.Mode = "login";
                ViewBag.Email = email;
                return View();
            }

            //_logger.LogInformation("Login success. UserId: {UserId}, Email: {Email}, Role: {Role}", data!.UserId, data.Email, data.UserType);

            // LƯU SESSION (giữ JWT cho API)
            HttpContext.Session.SetString("JWT", data!.Token);
            HttpContext.Session.SetInt32("UserId", data!.UserId);
            HttpContext.Session.SetString("Email", data.Email);
            HttpContext.Session.SetString("UserType", data.UserType.ToString());

            // COOKIE AUTHENTICATION (QUAN TRỌNG)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, data.UserId.ToString()),
                new Claim(ClaimTypes.Name, data.Email),          // dùng hiển thị header
                new Claim(ClaimTypes.Email, data.Email),
                new Claim(ClaimTypes.Role, data.UserType.ToString())
            };

            if (data.StudentId.HasValue)
            {
                claims.Add(new Claim(CustomJwtClaims.StudentId, data.StudentId.Value.ToString()));
            }

            if (data.ParentId.HasValue)
            {
                claims.Add(new Claim(CustomJwtClaims.ParentId, data.ParentId.Value.ToString()));
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                }
            );

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
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
                );

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ConfirmEmail(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return View("ConfirmEmailFailed");
            }

            // Gọi API backend
            var response = await _httpClient.GetAsync(
                $"{ApiConstant.apiBaseUrl}/api/auth/confirm-email?token={token}"
            );

            if (!response.IsSuccessStatusCode)
            {
                return View("ConfirmEmailFailed");
            }

            var result = await response.Content
                .ReadFromJsonAsync<ApiResponse<bool>>();

            if (result == null || !result.Success)
            {
                return View("ConfirmEmailFailed");
            }

            return View("ConfirmEmailSuccess");

        }
    }
}