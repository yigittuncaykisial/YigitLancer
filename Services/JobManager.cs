using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using System.Collections.Generic;

namespace Services
{
    public class JobManager : IJobService
    {
        private readonly IJobRepository _jobRepository;

        public JobManager(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public List<Jobs> GetAllJobs(int? categoryId) =>
            _jobRepository.GetAllJobs(categoryId);

        public Jobs GetJobById(int id) =>
            _jobRepository.GetJobById(id);

        public void CreateJob(Jobs job) =>
            _jobRepository.CreateJob(job);

        public void UpdateJob(Jobs job) => _jobRepository.UpdateJob(job);

        public void PurchaseJob(Jobs job, int buyerUserId)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));

            // Kullanıcı kendi ilanını satın alamaz
            if (job.UserId == buyerUserId)
                throw new InvalidOperationException("Kendi ilanınızı satın alamazsınız.");

            // Aynı ilan ikinci kez satın alınamaz
            if (job.IsPurchased)
                throw new InvalidOperationException("Bu ilan zaten satın alınmış.");

            job.IsPurchased = true;
            job.PurchasedByUserId = buyerUserId;
            _jobRepository.UpdateJob(job);
        }
        public List<Jobs> GetPurchasedJobsByUser(int userId) =>
            _jobRepository.GetPurchasedJobsByUser(userId);
        // Yeni:
        public List<Jobs> GetJobsByFreelancer(int freelancerUserId, bool includePurchased = true) =>
            _jobRepository.GetJobsByFreelancer(freelancerUserId, includePurchased);
    }

}

