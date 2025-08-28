using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace YigitLancer.Infrastructure
{
    public class ClaimUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection) =>
            connection.User?.FindFirst("UserId")?.Value
            ?? connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}