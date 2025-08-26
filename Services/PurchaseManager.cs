using System;
using System.Linq;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class PurchaseManager : IPurchaseService
    {
        private readonly IPurchaseRequestRepository _prRepo;
        private readonly IJobRepository _jobRepo;
        private readonly IChatRepository _chatRepo; 

        public PurchaseManager(IPurchaseRequestRepository prRepo, IJobRepository jobRepo, IChatRepository chatRepo)
        {
            _prRepo = prRepo;
            _jobRepo = jobRepo;
            _chatRepo = chatRepo;
        }

        public int CreateRequest(int jobId, int buyerUserId, string? note)
        {
            var job = _jobRepo.GetJobById(jobId);
            if (job == null) throw new InvalidOperationException("İlan bulunamadı.");
            if (job.IsPurchased) throw new InvalidOperationException("İlan zaten satılmış.");
            if (job.UserId == buyerUserId) throw new InvalidOperationException("Kendi ilanınızı satın alamazsınız.");

            var exists = _prRepo.GetPendingForJobAndBuyer(jobId, buyerUserId);
            if (exists != null) return exists.Id;

            var req = new PurchaseRequest
            {
                JobId = jobId,
                BuyerUserId = buyerUserId,
                Note = note
            };
            _prRepo.Create(req);
            return req.Id;
        }

        public void AcceptRequest(int requestId, int freelancerUserId)
        {
            var req = _prRepo.GetById(requestId);
            if (req == null || req.Status != PurchaseRequestStatus.Pending)
                throw new InvalidOperationException("Geçersiz talep.");

            var job = _jobRepo.GetJobById(req.JobId);
            if (job == null || job.UserId != freelancerUserId)
                throw new UnauthorizedAccessException("Bu talebi onaylama yetkiniz yok.");

            if (job.IsPurchased)
                throw new InvalidOperationException("İlan zaten satılmış.");

            // Satışı tamamla
            job.IsPurchased = true;
            job.PurchasedByUserId = req.BuyerUserId;
            _jobRepo.UpdateJob(job);

            // Talebi onayla
            req.Status = PurchaseRequestStatus.Accepted;
            req.DecidedAt = DateTime.UtcNow;
            _prRepo.Update(req);

            // Opsiyon: Aynı job için bekleyen diğer talepleri reddet
            // (İstersen burada topluca reddetme ekleyebiliriz)

            // Chat conversasyonu oluştur (varsa getir)
            _chatRepo.GetOrCreateConversation(job.Id, req.BuyerUserId, job.UserId);
        }

        public void RejectRequest(int requestId, int freelancerUserId)
        {
            var req = _prRepo.GetById(requestId);
            if (req == null || req.Status != PurchaseRequestStatus.Pending)
                throw new InvalidOperationException("Geçersiz talep.");

            var job = _jobRepo.GetJobById(req.JobId);
            if (job == null || job.UserId != freelancerUserId)
                throw new UnauthorizedAccessException("Bu talebi reddetme yetkiniz yok.");

            req.Status = PurchaseRequestStatus.Rejected;
            req.DecidedAt = DateTime.UtcNow;
            _prRepo.Update(req);
        }

        public System.Collections.Generic.List<PurchaseRequest> GetPendingForFreelancer(int freelancerUserId) =>
            _prRepo.GetPendingByFreelancer(freelancerUserId);

        public int CountPendingForFreelancer(int freelancerUserId) =>
            _prRepo.CountPendingForFreelancer(freelancerUserId);
    }
}
