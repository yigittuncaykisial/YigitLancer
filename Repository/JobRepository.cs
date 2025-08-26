using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly RepositoryContext _context;

        public JobRepository(RepositoryContext context)
        {
            _context = context;
        }

        public List<Jobs> GetAllJobs(int? categoryId)
        {
            var query = _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.User)
                .Include(j => j.PurchasedByUser)
                .Where(j => !j.IsPurchased)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(j => j.CategoryId == categoryId.Value);

            return query.ToList();
        }

        public Jobs GetJobById(int id)
        {
            return _context.Jobs
                .Include(j => j.User)
                .Include(j => j.Category)
                .Include(j => j.PurchasedByUser)
                .FirstOrDefault(j => j.Id == id);
        }

        public void CreateJob(Jobs job)
        {
            if (string.IsNullOrEmpty(job.JobImg))
                job.JobImg = "/uploads/default-job.png";

            _context.Jobs.Add(job);
            _context.SaveChanges();
        }

        public void UpdateJob(Jobs job)
        {
            _context.Jobs.Update(job);
            _context.SaveChanges();
        }

        public List<Jobs> GetPurchasedJobsByUser(int userId) =>
            _context.Jobs
                .AsNoTracking()
                .Where(j => j.IsPurchased && j.PurchasedByUserId == userId)
                .OrderByDescending(j => j.Id)
                .ToList();

        public List<Jobs> GetJobsByFreelancer(int freelancerUserId, bool includePurchased)
        {
            var query = _context.Jobs.AsQueryable();
            if (includePurchased)
                query = query.IgnoreQueryFilters(); // global filter yoksa etkisi olmaz

            return query.Where(j => j.UserId == freelancerUserId)
                        .Include(j => j.Category)
                        .AsNoTracking()
                        .OrderByDescending(j => j.Id)
                        .ToList();
        }
    }
}
