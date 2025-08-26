using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class CategoryManager : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryManager(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public List<Category> GetAllCategories() => _categoryRepository.GetAllCategories();

        public Category GetCategoryById(int id) => _categoryRepository.GetAllCategories().FirstOrDefault(c => c.Id == id);

        public void CreateCategory(Category category) => _categoryRepository.CreateCategory(category);

        public void UpdateCategory(Category category) => _categoryRepository.UpdateCategory(category);

        public void DeleteCategory(Category category) => _categoryRepository.DeleteCategory(category);
    }
}
