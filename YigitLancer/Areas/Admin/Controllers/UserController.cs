using Entities.Models;
using Microsoft.AspNetCore.Identity; // Şifreleme için gerekli
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Store1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Admin/User
        public IActionResult Index()
        {
            var users = _userService.GetAllUsers(false); // trackChanges false
            return View(users);
        }

        // GET: Admin/User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                // Şifreyi hashle
                var passwordHasher = new PasswordHasher<User>();
                user.UserPassword = passwordHasher.HashPassword(user, user.UserPassword);

                _userService.CreateUser(user);

                TempData["SuccessMessage"] = "User successfully created!";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // GET: Admin/User/Edit/5
        public IActionResult Edit(int id)
        {
            var user = _userService.GetUserById(id, false);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Admin/User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, User user)
        {
            if (id != user.UserId)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var existingUser = _userService.GetUserById(id, false);
                if (existingUser == null) return NotFound();

                // Şifreyi koru (edit sırasında değişmesin)
                user.UserPassword = existingUser.UserPassword;

                _userService.UpdateUser(user);
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // POST: Admin/User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _userService.DeleteUserAndRelated(id);
            TempData["Ok"] = "Kullanıcı ve ilişkili veriler silindi.";
            return RedirectToAction("Index"); // kullanıcı listesine dön
        }
    }
}
