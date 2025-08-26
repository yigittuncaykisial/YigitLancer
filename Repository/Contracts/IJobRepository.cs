using Entities.Models;
using System.Collections.Generic;

namespace Repositories.Contracts
{
    public interface IJobRepository
    {
        List<Jobs> GetAllJobs(int? categoryId);
        Jobs GetJobById(int id);
        void CreateJob(Jobs job);
        void UpdateJob(Jobs job);

        // YENİ:
        List<Jobs> GetPurchasedJobsByUser(int userId);

        // Yeni:
        List<Jobs> GetJobsByFreelancer(int freelancerUserId, bool includePurchased);
    }
}
