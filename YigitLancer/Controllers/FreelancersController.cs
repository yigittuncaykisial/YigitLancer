using Entities.Models;
using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Linq;

namespace Controllers
{
    public class FreelancersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IJobService _jobService;
        private readonly IReviewService _reviewService;

        public FreelancersController(IUserService userService, IJobService jobService, IReviewService reviewService)
        {
            _userService = userService;
            _jobService = jobService;
            _reviewService = reviewService;
        }

        // Tüm freelancerlar
        public IActionResult Index()
        {
            var freelancers = _userService.GetAllUsers(false)
                    .Where(u => !string.IsNullOrEmpty(u.UserJob) &&
                                u.UserJob.Trim().ToLower() == "freelancer")
                    .ToList();


            return View(freelancers);
        }

        // Freelancer detayı
        public IActionResult Details(int id)
        {
            var freelancer = _userService.GetUserById(id, false);
            if (freelancer == null) return NotFound();

            // .Query() YOK! Yeni servis metodunu kullanın:
            var jobs = _jobService.GetJobsByFreelancer(id, includePurchased: true);
            var reviews = _reviewService.GetReviewsByFreelancerId(id);

            var currentUser = _userService.GetUserByUsername(User?.Identity?.Name, false);
            ViewBag.CurrentUserId = currentUser?.UserId;

            var vm = new FreelancerDetailsViewModel
            {
                Freelancer = freelancer,
                Jobs = jobs,
                Reviews = reviews
            };

            return View(vm);
        }


        [HttpPost]
        public IActionResult AddReview(int freelancerId, int jobId, int rating, string comment)
        {
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName)) return Unauthorized();

            var reviewer = _userService.GetUserByUsername(userName);

            // Review nesnesi oluşturup mevcut CreateReview metodunu kullanıyoruz
            var review = new Review
            {
                FreelancerId = freelancerId,
                JobId = jobId,
                ReviewerId = reviewer.UserId,
                Rating = rating,
                Comment = comment
            };

            _reviewService.CreateReview(review);

            return RedirectToAction("Details", new { id = freelancerId });
        }
    }
}
