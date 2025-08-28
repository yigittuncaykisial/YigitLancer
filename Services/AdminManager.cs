using System.Linq;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Contracts;

namespace Services
{
    public class AdminManager : IAdminService
    {
        private readonly RepositoryContext _ctx;

        public AdminManager(RepositoryContext ctx)
        {
            _ctx = ctx;
        }

        public void DeleteUserAndRelated(int userId)
        {
            using var tx = _ctx.Database.BeginTransaction();

            // 1) Job'suz (pre-sale) sohbetler: buyer/freelancer user'a bağlı ve JobId == null
            var noJobConvs = _ctx.Conversations
                .Where(c => c.JobId == null && (c.BuyerUserId == userId || c.FreelancerUserId == userId))
                .Select(c => c.Id)
                .ToList();

            if (noJobConvs.Count > 0)
            {
                var msgs = _ctx.Messages.Where(m => noJobConvs.Contains(m.ConversationId)).ToList();
                if (msgs.Count > 0) _ctx.Messages.RemoveRange(msgs);

                var convs = _ctx.Conversations.Where(c => noJobConvs.Contains(c.Id)).ToList();
                _ctx.Conversations.RemoveRange(convs);
                _ctx.SaveChanges();
            }

            // 2) Bu kullanıcının açtığı işler -> Cascade ile conv, msg, review, pr zaten silinir
            var myJobs = _ctx.Jobs.Where(j => j.UserId == userId).ToList();
            if (myJobs.Count > 0)
            {
                _ctx.Jobs.RemoveRange(myJobs);
                _ctx.SaveChanges();
            }

            // 3) Bu kullanıcının alıcı olduğu işlerde PurchasedByUserId = null
            var purchasedByUser = _ctx.Jobs.Where(j => j.PurchasedByUserId == userId).ToList();
            if (purchasedByUser.Count > 0)
            {
                foreach (var j in purchasedByUser)
                    j.PurchasedByUserId = null;

                _ctx.SaveChanges();
            }

            // 4) Bu kullanıcının yaptığı reviewlar (reviewer olarak)
            var myReviews = _ctx.Reviews.Where(r => r.ReviewerId == userId).ToList();
            if (myReviews.Count > 0)
            {
                _ctx.Reviews.RemoveRange(myReviews);
                _ctx.SaveChanges();
            }

            // 5) Bu kullanıcının buyer olduğu purchase requests
            var myPurchaseReqs = _ctx.PurchaseRequests.Where(p => p.BuyerUserId == userId).ToList();
            if (myPurchaseReqs.Count > 0)
            {
                _ctx.PurchaseRequests.RemoveRange(myPurchaseReqs);
                _ctx.SaveChanges();
            }

            // 6) Son olarak kullanıcı
            var user = _ctx.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                _ctx.Users.Remove(user);
                _ctx.SaveChanges();
            }

            tx.Commit();
        }
    }
}
