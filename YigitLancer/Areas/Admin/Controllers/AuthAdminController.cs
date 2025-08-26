using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Services.Contracts;
using System.Security.Claims;

namespace YigitLancer.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthAdminController : Controller
    {
        private readonly IUserService _userService;

        public AuthAdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Admin login sayfası
        }

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password)
        {
            var user = _userService.Login(username, password);

            if (user != null && user.IsAdmin) // Sadece admin giriş yapabilir
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("IsAdmin", user.IsAdmin.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            ViewBag.Error = "Invalid username or password or not admin.";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Direkt olarak /Home'a yönlendir
            return Redirect("/Home");
        }


        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
