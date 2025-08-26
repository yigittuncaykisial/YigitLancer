using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Entities.Models;

namespace YigitLancer.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IJobService _jobService;

        public ReviewController(IReviewService reviewService, IJobService jobService) // EKLENDİ
        {
            _reviewService = reviewService;
            _jobService = jobService; // EKLENDİ
        }

        // Tüm review’leri listele
        public IActionResult Index()
        {
            // Burada varsayımsal olarak GetReviewsByFreelancerId(0) ile tüm review’leri alıyoruz
            var reviews = _reviewService.GetReviewsByFreelancerId(0); // 0 veya özel bir işaret tüm kullanıcıları alacak şekilde repository’de ele al
            return View(reviews);
        }

        // Freelancer’a ait review’leri göster
        public IActionResult ByFreelancer(int freelancerId)
        {
            var reviews = _reviewService.GetReviewsByFreelancerId(freelancerId);
            return View(reviews);
        }

        // Yeni review ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddReview(int freelancerId, int jobId, int reviewerId, int rating, string comment)
        {
            if (rating < 1 || rating > 5) return BadRequest("Geçersiz puan.");

            var job = _jobService.GetJobById(jobId);
            if (job == null || job.UserId != freelancerId) return BadRequest("İş bulunamadı veya freelancera ait değil.");
            if (!job.IsPurchased || job.PurchasedByUserId != reviewerId) return Forbid();

            _reviewService.CreateReview(new Review
            {
                FreelancerId = freelancerId,
                JobId = jobId,
                ReviewerId = reviewerId,
                Rating = rating,
                Comment = comment
            });

            return RedirectToAction("Details", "Freelancers", new { id = freelancerId });
        }
    }
}
