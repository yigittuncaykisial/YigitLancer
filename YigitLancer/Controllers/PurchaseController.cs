using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Entities.Models;

[Authorize] // checkout'a sadece login kullanıcı girsin
public class PurchaseController : Controller
{
    private readonly IJobService _jobService;
    private readonly IUserService _userService;
    private readonly IPurchaseService _purchaseService;
    private readonly IChatService _chatService;

    public PurchaseController(IJobService jobService, IUserService userService, IPurchaseService purchaseService,IChatService chatService)
    {
        _jobService = jobService;
        _userService = userService;
        _purchaseService = purchaseService;
        _chatService = chatService;
    }

    [HttpGet]
    public IActionResult Checkout(int jobId)
    {
        var job = _jobService.GetJobById(jobId);
        if (job == null) return NotFound();

        var currentUser = GetCurrentUser();
        if (currentUser == null)
        {
            TempData["Error"] = "Devam etmek için giriş yapın.";
            return RedirectToAction("Index", "Auth");
        }

        if (job.UserId == currentUser.UserId)
        {
            TempData["Error"] = "Kendi ilanınızı satın alamazsınız.";
            return RedirectToAction("Details", "Jobs", new { id = jobId });
        }
        if (job.IsPurchased)
        {
            TempData["Error"] = "Bu ilan zaten satın alınmış.";
            return RedirectToAction("Details", "Jobs", new { id = jobId });
        }

        return View(job);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Checkout(int jobId, string cardNumber, string expiry, string cvc)
    {
        var job = _jobService.GetJobById(jobId);
        if (job == null) return NotFound();

        var currentUser = GetCurrentUser();
        if (currentUser == null)
        {
            TempData["Error"] = "Kullanıcı bulunamadı.";
            return RedirectToAction("Checkout", new { jobId });
        }

        // UI manipülasyonlarına karşı tekrar kontrol
        if (job.UserId == currentUser.UserId)
        {
            TempData["Error"] = "Kendi ilanınızı satın alamazsınız.";
            return RedirectToAction("Details", "Jobs", new { id = jobId });
        }
        if (job.IsPurchased)
        {
            TempData["Error"] = "Bu ilan zaten satın alınmış.";
            return RedirectToAction("Details", "Jobs", new { id = jobId });
        }

        // Basit kart doğrulama (opsiyonel)
        if (string.IsNullOrWhiteSpace(cardNumber) ||
            string.IsNullOrWhiteSpace(expiry) ||
            string.IsNullOrWhiteSpace(cvc))
        {
            TempData["Error"] = "Ödeme bilgilerini eksiksiz girin.";
            return RedirectToAction("Checkout", new { jobId });
        }

        try
        {
            // Talep oluştur (senin var olan servisin)
            var requestId = _purchaseService.CreateRequest(jobId, currentUser.UserId, note: null);

            // Alıcı = currentUser, Freelancer = job.UserId → sohbeti hazırla
            var conv = _chatService.GetOrCreateConversation(
                buyerUserId: currentUser.UserId,
                freelancerUserId: job.UserId,
                jobId: job.Id
            );

            // (İsteğe bağlı) otomatik ilk sistem mesajı
            _chatService.SendMessage(conv.Id, currentUser.UserId,
                $"Satın alma talebi oluşturuldu. Talep No: #{requestId}");

            TempData["Success"] = "Satın alma talebiniz freelancer onayına gönderildi. Sohbet açıldı.";
            return RedirectToAction("Conversation", "Chat", new { id = conv.Id }); // ← ARTIK VAR
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Details", "Jobs", new { id = jobId });
        }
    }


    [HttpGet]
    public IActionResult Index()
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return RedirectToAction("Index", "Auth");

        var purchasedJobs = _jobService.GetPurchasedJobsByUser(currentUser.UserId);
        return View(purchasedJobs);
    }

    private User GetCurrentUser()
    {
        var userName = User?.Identity?.Name;
        if (string.IsNullOrEmpty(userName)) return null;
        return _userService.GetUserByUsername(userName, false);
    }


}
