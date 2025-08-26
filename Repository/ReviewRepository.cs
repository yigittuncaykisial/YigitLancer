using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly RepositoryContext _context;

        public ReviewRepository(RepositoryContext context)
        {
            _context = context;
        }

        public IEnumerable<Review> GetReviewsByFreelancerId(int freelancerId)
        {
            return _context.Reviews
                           .Where(r => r.FreelancerId == freelancerId)
                           .ToList();
        }



        public void CreateReview(Review review)
        {
            _context.Reviews.Add(review);
        }
        public IEnumerable<Review> GetByFreelancerIdWithUserAndJob(int freelancerId)
        {
            return _context.Reviews
                .Include(r => r.Reviewer)    // Reviewer bilgisi
                .Include(r => r.Job)         // Job bilgisi
                .Where(r => r.FreelancerId == freelancerId)
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking()
                .ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
