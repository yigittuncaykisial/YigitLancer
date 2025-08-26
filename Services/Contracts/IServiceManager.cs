using Services.Contracts;

namespace Services.Contracts
{
    public interface IServiceManager
    {
        IUserService UserService { get; }
        ICategoryService CategoryService { get; }
        IJobService JobService { get; }
        IReviewService ReviewService { get; }
    }
}
