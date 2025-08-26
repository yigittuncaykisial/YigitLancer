using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Repositories.Contracts;

namespace Repositories
{
    public class PurchaseRequestRepository : IPurchaseRequestRepository
    {
        private readonly RepositoryContext _ctx;
        public PurchaseRequestRepository(RepositoryContext ctx) => _ctx = ctx;

        public PurchaseRequest GetById(int id) =>
            _ctx.PurchaseRequests
                .Include(p => p.Job).ThenInclude(j => j.User)
                .Include(p => p.Buyer)
                .FirstOrDefault(x => x.Id == id);

        public PurchaseRequest GetPendingForJobAndBuyer(int jobId, int buyerUserId) =>
            _ctx.PurchaseRequests.FirstOrDefault(x => x.JobId == jobId && x.BuyerUserId == buyerUserId && x.Status == PurchaseRequestStatus.Pending);

        public void Create(PurchaseRequest req)
        {
            _ctx.PurchaseRequests.Add(req);
            _ctx.SaveChanges();
        }

        public void Update(PurchaseRequest req)
        {
            _ctx.PurchaseRequests.Update(req);
            _ctx.SaveChanges();
        }

        public List<PurchaseRequest> GetPendingByFreelancer(int freelancerUserId) =>
            _ctx.PurchaseRequests
                .Include(p => p.Job)
                .Include(p => p.Buyer)
                .Where(p => p.Job.UserId == freelancerUserId && p.Status == PurchaseRequestStatus.Pending)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

        public int CountPendingForFreelancer(int freelancerUserId) =>
            _ctx.PurchaseRequests.Count(p => p.Job.UserId == freelancerUserId && p.Status == PurchaseRequestStatus.Pending);
    }
}