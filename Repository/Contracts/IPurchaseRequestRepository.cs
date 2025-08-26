using System.Collections.Generic;
using Entities.Models;

namespace Repositories.Contracts
{
    public interface IPurchaseRequestRepository
    {
        PurchaseRequest GetById(int id);
        void Create(PurchaseRequest req);
        void Update(PurchaseRequest req);
        List<PurchaseRequest> GetPendingByFreelancer(int freelancerUserId);
        int CountPendingForFreelancer(int freelancerUserId);
        PurchaseRequest GetPendingForJobAndBuyer(int jobId, int buyerUserId);
    }
}
