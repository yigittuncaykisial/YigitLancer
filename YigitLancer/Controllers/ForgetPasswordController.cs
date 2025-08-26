using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace YigitLancer.Controllers
{
    public class ForgetPasswordController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public ForgetPasswordController(IUserService userService, IPasswordHasher<User> passwordHasher)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string emailOrUsername)
        {
            var user = _userService.GetUserByUsername(emailOrUsername) ??
                       _userService.GetUserByEmail(emailOrUsername);

            if (user == null)
            {
                ViewBag.Error = "User not found.";
                return View();
            }

            // TODO: send password reset link via email
            TempData["Info"] = "Password reset link has been sent to your email.";
            return RedirectToAction("Index", "Auth");
        }

        [HttpGet]
        public IActionResult ResetPassword(int userId)
        {
            var user = _userService.GetUserById(userId, false);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult ResetPassword(int userId, string newPassword)
        {
            var user = _userService.GetUserById(userId, false);
            if (user == null)
                return NotFound();

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 8)
            {
                ModelState.AddModelError("UserPassword", "Password must be at least 8 characters.");
                return View(user);
            }

            user.UserPassword = _passwordHasher.HashPassword(user, newPassword);
            _userService.UpdateUser(user);

            TempData["Success"] = "Password has been changed successfully!";
            return RedirectToAction("Index", "Auth");
        }
    }
}
