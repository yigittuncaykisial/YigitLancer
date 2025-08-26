using Repositories.Contracts;

namespace Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _context;
        private IUserRepository _userRepository;
        private ICategoryRepository _categoryRepository;
        private IJobRepository _jobRepository;
        private IReviewRepository _reviewRepository;

        public RepositoryManager(RepositoryContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository =>
            _userRepository ??= new UserRepository(_context);

        public ICategoryRepository CategoryRepository =>
            _categoryRepository ??= new CategoryRepository(_context);

        public IJobRepository JobRepository =>
            _jobRepository ??= new JobRepository(_context);

        public IReviewRepository ReviewRepository =>
            _reviewRepository ??= new ReviewRepository(_context);

        public void Save() => _context.SaveChanges();
    }
}
