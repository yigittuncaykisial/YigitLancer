using Entities.Models;
using System.Collections.Generic;

namespace Repositories.Contracts
{
    public interface ICategoryRepository
    {
        List<Category> GetAllCategories();
        void CreateCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
    }
}
