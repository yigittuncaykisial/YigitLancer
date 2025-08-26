using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using System.Collections.Generic;

namespace Services
{
    public class ReviewManager : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewManager(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public IEnumerable<Review> GetReviewsByFreelancerId(int freelancerId)
        {
            return _reviewRepository.GetReviewsByFreelancerId(freelancerId);
        }

        public void CreateReview(Review review)
        {
            _reviewRepository.CreateReview(review);
            _reviewRepository.Save();
        }
    }
}
