using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

[Route("api/purchase")]
[ApiController]
public class PurchaseApiController : ControllerBase
{
    private readonly IPurchaseService _purchase; private readonly IUserService _users;
    public PurchaseApiController(IPurchaseService p, IUserService u) { _purchase = p; _users = u; }

    [HttpGet("pendingCount")]
    public IActionResult PendingCount()
    {
        var name = User?.Identity?.Name;
        if (string.IsNullOrEmpty(name)) return Ok(new { count = 0 });
        var me = _users.GetUserByUsername(name, false);
        if (me == null) return Ok(new { count = 0 });
        var c = _purchase.CountPendingForFreelancer(me.UserId);
        return Ok(new { count = c });
    }
}
