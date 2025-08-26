using Entities.Models;
using System.Collections.Generic;

namespace Services.Contracts
{
    public interface IReviewService
    {
        IEnumerable<Review> GetReviewsByFreelancerId(int freelancerId);
        void CreateReview(Review review);
    }
}
