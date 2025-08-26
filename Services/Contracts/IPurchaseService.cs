using System.Collections.Generic;
using Entities.Models;

namespace Services.Contracts
{
    public interface IPurchaseService
    {
        int CreateRequest(int jobId, int buyerUserId, string? note);
        List<PurchaseRequest> GetPendingForFreelancer(int freelancerUserId);
        int CountPendingForFreelancer(int freelancerUserId);
        void AcceptRequest(int requestId, int freelancerUserId);
        void RejectRequest(int requestId, int freelancerUserId);
    }
}
