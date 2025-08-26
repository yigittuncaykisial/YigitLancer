using Entities.Models;
using System.Collections.Generic;

namespace Repositories.Contracts
{
    public interface IReviewRepository
    {
       IEnumerable<Review> GetReviewsByFreelancerId(int freelancerId);
        void CreateReview(Review review);
        void Save();

        IEnumerable<Review> GetByFreelancerIdWithUserAndJob(int freelancerId);
    }
}
