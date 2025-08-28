using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Diagnostics;
using YigitLancer.Models;

namespace YigitLancer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IJobService _jobService; // EKLENDI

        public HomeController(ILogger<HomeController> logger, IJobService jobService) // DI
        {
            _logger = logger;
            _jobService = jobService;
        }

        public IActionResult Index()
        {
            // Tüm kategorilerdeki iþleri getir
            var jobs = _jobService.GetAllJobs(null);

            return View(jobs);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
