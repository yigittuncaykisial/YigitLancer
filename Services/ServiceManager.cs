using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Repositories.Contracts;
using Services;
using Services.Contracts;

public class ServiceManager : IServiceManager
{
    private readonly IUserService _userService;
    private readonly ICategoryService _categoryService;
    private readonly IJobService _jobService;
    private readonly IReviewService _reviewService;

    public ServiceManager(IRepositoryManager repositoryManager, IPasswordHasher<User> passwordHasher)
    {
        _userService = new UserManager(repositoryManager.UserRepository, passwordHasher);
        _categoryService = new CategoryManager(repositoryManager.CategoryRepository);
        _jobService = new JobManager(repositoryManager.JobRepository);
        _reviewService = new ReviewManager(repositoryManager.ReviewRepository);
    }

    public IUserService UserService => _userService;
    public ICategoryService CategoryService => _categoryService;
    public IJobService JobService => _jobService;
    public IReviewService ReviewService => _reviewService;
}
