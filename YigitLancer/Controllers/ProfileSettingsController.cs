using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Entities.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace YigitLancer.Controllers
{
    public class ProfileSettingsController : Controller
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _env;
        private readonly IPasswordHasher<User> _passwordHasher; // <-- EKLE

        public ProfileSettingsController(
            IUserService userService,
            IWebHostEnvironment env,
            IPasswordHasher<User> passwordHasher) // <-- EKLE
        {
            _userService = userService;
            _env = env;
            _passwordHasher = passwordHasher; // <-- EKLE
        }

        [HttpGet]
        public IActionResult Index()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Auth");

            var user = _userService.GetUserByUsername(username);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User model, IFormFile? profileImage)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Auth");

            var user = _userService.GetUserByUsername(username);
            if (user == null)
                return NotFound();

            // Profil alanlarını güncelle
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.UserName = model.UserName;
            user.UserEmail = model.UserEmail;
            user.UserDescription = model.UserDescription;
            user.Age = model.Age;

            // Profil resmi
            if (profileImage != null && profileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    profileImage.CopyTo(fileStream);
                }

                user.ProfileImagePath = "/uploads/" + uniqueFileName;
            }

            _userService.UpdateUser(user);
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }

        // ŞİFRE DEĞİŞTİRME — YENİ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Auth");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen formu kontrol edin.";
                return RedirectToAction("Index");
            }

            var user = _userService.GetUserByUsername(username);
            if (user == null)
                return NotFound();

            // Mevcut şifreyi doğrula
            var verify = _passwordHasher.VerifyHashedPassword(user, user.UserPassword, model.CurrentPassword);
            if (verify != PasswordVerificationResult.Success)
            {
                TempData["Error"] = "Mevcut şifre hatalı.";
                return RedirectToAction("Index");
            }

            if (model.NewPassword == model.CurrentPassword)
            {
                TempData["Error"] = "Yeni şifre mevcut şifre ile aynı olamaz.";
                return RedirectToAction("Index");
            }

            if (model.NewPassword.Length < 8)
            {
                TempData["Error"] = "Yeni şifre en az 8 karakter olmalı.";
                return RedirectToAction("Index");
            }

            // Yeni şifreyi hashle ve kaydet
            user.UserPassword = _passwordHasher.HashPassword(user, model.NewPassword);
            _userService.UpdateUser(user);

            // (Öneri) güvenlik için yeniden giriş iste
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Şifreniz güncellendi. Lütfen tekrar giriş yapın.";
            return RedirectToAction("Index", "Auth");
        }
    }
}
