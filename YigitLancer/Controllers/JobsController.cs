using Entities.Models;
using Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System;
using System.IO;
using System.Linq;

namespace Controllers
{
    public class JobsController : Controller
    {
        private readonly IJobService _jobService;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _webHost;

        public JobsController(IJobService jobService, ICategoryService categoryService, IUserService userService, IWebHostEnvironment webHost)
        {
            _jobService = jobService;
            _categoryService = categoryService;
            _userService = userService;
            _webHost = webHost;
        }

        public IActionResult Index(int? categoryId)
        {
            var jobs = _jobService.GetAllJobs(categoryId);

            // Debug için - sadece geliştirme ortamında
            foreach (var job in jobs.Take(3))
            {
                System.Diagnostics.Debug.WriteLine($"Job {job.Id}: User is null? {job.User == null}");
                if (job.User != null)
                {
                    System.Diagnostics.Debug.WriteLine($"User {job.User.UserId}: {job.User.Name} {job.User.Surname}");
                }
            }

            ViewData["Environment"] = "Development"; // Debug bilgileri için
            return View(jobs);
        }

        public IActionResult Details(int id)
        {
            var job = _jobService.GetJobById(id);
            if (job == null) return NotFound();

            var owner = _userService.GetUserById(job.UserId, false);
            ViewBag.Owner = owner;

            // ReviewerId’yi view’da kullanmak istersen:
            var currentUser = _userService.GetUserByUsername(User?.Identity?.Name, false);
            ViewBag.ReviewerId = currentUser?.UserId;

            return View(job);
        }
// ...

[Authorize(Policy = "FreelancerOnly")]
    public IActionResult Create()
    {
        ViewBag.Categories = _categoryService.GetAllCategories();
        return View(new JobCreateViewModel());
    }

    [HttpPost]
    [Authorize(Policy = "FreelancerOnly")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(JobCreateViewModel vm)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return RedirectToAction("Index", "Auth"); // güvenlik

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = _categoryService.GetAllCategories();
            return View(vm);
        }

        // Görsel kaydetme + map + create (senin kodun)
        string imagePath = "/uploads/default-job.png";
        if (vm.JobImgFile is { Length: > 0 })
        {
            var uploadsFolder = Path.Combine(_webHost.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(vm.JobImgFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using var fs = System.IO.File.Create(filePath);
            vm.JobImgFile.CopyTo(fs);
            imagePath = "/uploads/" + uniqueFileName;
        }

        var job = new Jobs
        {
            JobName = vm.JobName.Trim(),
            JobDescription = vm.JobDescription.Trim(),
            JobPrice = vm.JobPrice,
            CategoryId = vm.CategoryId!.Value,
            JobImg = imagePath,
            UserId = currentUser.UserId,
            IsPurchased = false
        };

        _jobService.CreateJob(job);
        TempData["Success"] = "İlan başarıyla eklendi!";
        return RedirectToAction("Index");
    }




    private User GetCurrentUser()
        {
            var userName = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return null;

            return _userService.GetUserByUsername(userName, false);
        }

        // === EDIT ===
        [Authorize(Policy = "FreelancerOnly")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var job = _jobService.GetJobById(id);
            if (job == null) return NotFound();

            var currentUser = GetCurrentUser();
            if (currentUser == null) return RedirectToAction("Index", "Auth");

            // Sadece ilan sahibi düzenleyebilir
            if (job.UserId != currentUser.UserId)
            {
                TempData["Error"] = "Bu ilanı düzenleme yetkiniz yok.";
                return RedirectToAction("Details", new { id });
            }

            // Satılmış işi düzenlemeyi engelle (isterseniz kaldırabilirsiniz)
            if (job.IsPurchased)
            {
                TempData["Error"] = "Satılmış bir ilanı düzenleyemezsiniz.";
                return RedirectToAction("Details", new { id });
            }

            // Formu doldurmak için VM
            ViewBag.Categories = _categoryService.GetAllCategories();

            var vm = new Entities.ViewModels.JobCreateViewModel
            {
                JobName = job.JobName,
                JobDescription = job.JobDescription,
                JobPrice = job.JobPrice,
                CategoryId = job.CategoryId
                // JobImgFile yok; mevcut görseli sayfada sadece önizleme olarak göstereceğiz
            };

            ViewBag.CurrentImage = string.IsNullOrEmpty(job.JobImg) ? "/uploads/default-job.png" : job.JobImg;
            ViewBag.JobId = job.Id;

            return View(vm);
        }

        [Authorize(Policy = "FreelancerOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Entities.ViewModels.JobCreateViewModel vm)
        {
            var job = _jobService.GetJobById(id);
            if (job == null) return NotFound();

            var currentUser = GetCurrentUser();
            if (currentUser == null) return RedirectToAction("Index", "Auth");

            // Sahiplik kontrolü
            if (job.UserId != currentUser.UserId)
            {
                TempData["Error"] = "Bu ilanı düzenleme yetkiniz yok.";
                return RedirectToAction("Details", new { id });
            }

            // Satılmış işi düzenlemeyi engelle (isterseniz kaldırabilirsiniz)
            if (job.IsPurchased)
            {
                TempData["Error"] = "Satılmış bir ilanı düzenleyemezsiniz.";
                return RedirectToAction("Details", new { id });
            }

            // Server-side validation
            if (vm.CategoryId == null) ModelState.AddModelError(nameof(vm.CategoryId), "Kategori seçiniz.");
            if (string.IsNullOrWhiteSpace(vm.JobName)) ModelState.AddModelError(nameof(vm.JobName), "İlan adı zorunludur.");
            if (string.IsNullOrWhiteSpace(vm.JobDescription)) ModelState.AddModelError(nameof(vm.JobDescription), "Açıklama zorunludur.");
            if (vm.JobPrice <= 0) ModelState.AddModelError(nameof(vm.JobPrice), "Fiyat 0’dan büyük olmalıdır.");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryService.GetAllCategories();
                ViewBag.CurrentImage = string.IsNullOrEmpty(job.JobImg) ? "/uploads/default-job.png" : job.JobImg;
                ViewBag.JobId = job.Id;
                return View(vm);
            }

            // Görsel güncellenecekse kaydet
            if (vm.JobImgFile is { Length: > 0 })
            {
                var uploadsFolder = Path.Combine(_webHost.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(vm.JobImgFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var fs = System.IO.File.Create(filePath);
                vm.JobImgFile.CopyTo(fs);
                job.JobImg = "/uploads/" + uniqueFileName;
            }

            // Alanları güncelle
            job.JobName = vm.JobName.Trim();
            job.JobDescription = vm.JobDescription.Trim();
            job.JobPrice = vm.JobPrice;
            job.CategoryId = vm.CategoryId!.Value;

            _jobService.UpdateJob(job);

            TempData["Success"] = "İlan başarıyla güncellendi.";
            return RedirectToAction("Details", new { id = job.Id });
        }

    }
}
