using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Services.Contracts;
using System.Security.Claims;
using Entities.Models;
using Entities.ViewModels;

namespace YigitLancer.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel vm)
        {
            // 1) Server-side validation
            if (!ModelState.IsValid)
                return View(vm);

            // 2) Giriş dene
            var user = _userService.Login(vm.Username, vm.Password);
            if (user == null)
            {
                // Genel mesaj (kullanıcı adı/şifre ayrımı yapma → enumeration’a karşı)
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(vm);
            }

            // 3) Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("IsAdmin", user.IsAdmin.ToString()),
                new Claim("UserJob", (user.UserJob ?? "Buyer").Trim()),
                new Claim("ProfileImagePath", string.IsNullOrEmpty(user.ProfileImagePath)
                                              ? "/uploads/default.png"
                                              : user.ProfileImagePath)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = vm.RememberMe,
                    // ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) // istersen açık oturum süresi
                });

            return user.IsAdmin
                ? RedirectToAction("Index", "Admin", new { area = "Admin" })
                : RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() => View();
    }
}
