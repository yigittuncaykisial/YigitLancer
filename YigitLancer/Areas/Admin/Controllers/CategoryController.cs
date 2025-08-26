using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Entities.Models;
using Services.Contracts;
using Entities.DTOs;

namespace YigitLancer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: /Admin/Category
        public IActionResult Index()
        {
            var categories = _categoryService.GetAllCategories().Select(c => new CategoryDtos
            {
                Id = c.Id,
                CategoryName = c.CategoryName
            })
                .ToList();
            return View(categories);
        }



        // GET: /Admin/Category/Create
        [HttpGet]
        public IActionResult Create() => View(new CategoryDtos());

        // POST: /Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryDtos dto)
        {
            dto.CategoryName = dto.CategoryName?.Trim();

            if (string.IsNullOrWhiteSpace(dto.CategoryName))
                ModelState.AddModelError(nameof(dto.CategoryName), "Kategori adı zorunludur.");
            else if (dto.CategoryName.Length > 50)
                ModelState.AddModelError(nameof(dto.CategoryName), "Kategori adı en fazla 50 karakter olmalıdır.");

            var exists = _categoryService
                .GetAllCategories()
                .Any(c => c.CategoryName.ToLower() == dto.CategoryName.ToLower());

            if (exists)
                ModelState.AddModelError(nameof(dto.CategoryName), "Bu kategori adı zaten mevcut.");

            if (!ModelState.IsValid)
                return View(dto);

            var entity = new Category
            {
                CategoryName = dto.CategoryName
            };

            _categoryService.CreateCategory(entity);
            TempData["Success"] = "Kategori eklendi.";
            return RedirectToAction(nameof(Index));
        }


        // GET: /Admin/Category/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var abc = _categoryService.GetCategoryById(id);
            if (abc == null) return NotFound();


            var dto = new CategoryDtos
            {
                Id = abc.Id,
                CategoryName = abc.CategoryName
            };

            return View(dto);
        }

        // POST: /Admin/Category/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryDtos dto)
        {
            dto.CategoryName = dto.CategoryName?.Trim();

            if (string.IsNullOrWhiteSpace(dto.CategoryName))
                ModelState.AddModelError(nameof(dto.CategoryName), "Kategori adı zorunludur.");
            else if (dto.CategoryName.Length > 50)
                ModelState.AddModelError(nameof(dto.CategoryName), "Kategori adı en fazla 50 karakter olmalıdır.");

            if (!ModelState.IsValid)
                return View(dto);

            var exists = _categoryService.GetCategoryById(dto.Id);
            if (exists == null) return NotFound();


            exists.CategoryName = dto.CategoryName;

            _categoryService.UpdateCategory(exists);
            TempData["Success"] = "Kategori güncellendi.";
            return RedirectToAction(nameof(Index));
        }
        // GET: /Admin/Category/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var cba = _categoryService.GetCategoryById(id);
            if (cba == null) return NotFound();


            var dto = new CategoryDtos
            {
                Id = cba.Id,
                CategoryName = cba.CategoryName
            };

            return View(dto);
        }

        // POST: /Admin/Category/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(CategoryDtos dto)
        {
            var entity = _categoryService.GetCategoryById(dto.Id);
            if (entity == null) return NotFound();

            _categoryService.DeleteCategory(entity);
            TempData["Success"] = "Kategori silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
