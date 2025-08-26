using Entities.Models;
using System.Collections.Generic;

namespace Services.Contracts
{
    public interface IJobService
    {
        List<Jobs> GetAllJobs(int? categoryId);
        Jobs GetJobById(int id);
        void CreateJob(Jobs job);
        void PurchaseJob(Jobs job, int buyerUserId);

        // YENİ:
        List<Jobs> GetPurchasedJobsByUser(int userId);

        // Yeni: Freelancer'ın tüm işlerini getir (satılmışlar dahil)
        List<Jobs> GetJobsByFreelancer(int freelancerUserId, bool includePurchased = true);
    }
}
