using Entities.Models;
using Repositories.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly RepositoryContext _context;

        public CategoryRepository(RepositoryContext context)
        {
            _context = context;
        }

        public List<Category> GetAllCategories() => _context.Categories.ToList();

        public void CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }

        public void DeleteCategory(Category category)
        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }
    }
}
