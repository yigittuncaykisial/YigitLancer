using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Services.Contracts;
using Entities.Models;
using Entities.ViewModels;

namespace YigitLancer.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _env;

        public RegisterController(IUserService userService, IWebHostEnvironment env)
        {
            _userService = userService;
            _env = env;
        }

        [HttpGet]
        public IActionResult Index() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(RegisterViewModel vm)
        {
            // 1) DataAnnotations
            if (!ModelState.IsValid) return View(vm);

            // 2) Basit captcha
            if (vm.Captcha.Trim() != "2")
            {
                ModelState.AddModelError(nameof(vm.Captcha), "Captcha yanlış.");
                return View(vm);
            }

            // 3) Benzersizlik
            if (_userService.ExistsByUserName(vm.UserName))
            {
                ModelState.AddModelError(nameof(vm.UserName), "Kullanıcı adı zaten alınmış.");
                return View(vm);
            }
            if (_userService.ExistsByEmail(vm.UserEmail))
            {
                ModelState.AddModelError(nameof(vm.UserEmail), "E-posta zaten kayıtlı.");
                return View(vm);
            }

            // 4) Profil görseli
            string profilePath = "/uploads/default.png";
            if (vm.ProfileImage is { Length: > 0 })
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.ProfileImage.FileName)}";
                var physical = Path.Combine(_env.WebRootPath, "uploads", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(physical)!);
                using var fs = System.IO.File.Create(physical);
                vm.ProfileImage.CopyTo(fs);
                profilePath = $"/uploads/{fileName}";
            }

            // 5) Map (hash'i service yapacak)
            var user = new User
            {
                Name = vm.Name,
                Surname = vm.Surname,
                UserName = vm.UserName,
                UserEmail = vm.UserEmail,
                UserPassword = vm.UserPassword, // düz şifre – service hashler
                Age = vm.Age,
                UserJob = vm.UserJob,
                UserDescription = vm.UserDescription,
                ProfileImagePath = profilePath
            };

            _userService.CreateUser(user);

            TempData["Success"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
            return RedirectToAction("Index", "Auth");
        }
    }
}
