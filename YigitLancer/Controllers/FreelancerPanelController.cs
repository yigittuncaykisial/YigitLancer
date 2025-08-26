using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Contracts;

[Authorize(Policy = "FreelancerOnly")]
public class FreelancerPanelController : Controller
{
    private readonly IJobService _jobService;
    private readonly IPurchaseService _purchaseService;
    private readonly IUserService _userService;

    public FreelancerPanelController(IJobService jobService, IPurchaseService purchaseService, IUserService userService)
    {
        _jobService = jobService;
        _purchaseService = purchaseService;
        _userService = userService;
    }

    private Entities.Models.User GetCurrentUser()
    {
        var u = User?.Identity?.Name;
        return string.IsNullOrEmpty(u) ? null : _userService.GetUserByUsername(u, false);
    }

    public IActionResult MyJobs()
    {
        var me = GetCurrentUser();
        if (me == null) return RedirectToAction("Index", "Auth");
        var jobs = _jobService.GetJobsByFreelancer(me.UserId, includePurchased: true);
        return View(jobs); // Satışta/Satıldı etiketi view'de
    }

    public IActionResult Pending()
    {
        var me = GetCurrentUser();
        if (me == null) return RedirectToAction("Index", "Auth");
        var list = _purchaseService.GetPendingForFreelancer(me.UserId);
        return View(list);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Accept(int id)
    {
        var me = GetCurrentUser();
        if (me == null) return RedirectToAction("Index", "Auth");
        try
        {
            _purchaseService.AcceptRequest(id, me.UserId);
            TempData["Success"] = "Talep onaylandı ve satış tamamlandı.";
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction("Pending");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Reject(int id)
    {
        var me = GetCurrentUser();
        if (me == null) return RedirectToAction("Index", "Auth");
        try
        {
            _purchaseService.RejectRequest(id, me.UserId);
            TempData["Success"] = "Talep reddedildi.";
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction("Pending");
    }
}
