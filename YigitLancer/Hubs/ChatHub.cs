using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace YigitLancer.Hubs   // <-- web projenin namespace'ine göre düzelt
{
    [Authorize]
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var uid = Context.User?.FindFirstValue("UserId");
            if (!string.IsNullOrEmpty(uid))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{uid}");
            }
            await base.OnConnectedAsync();
        }

        public Task JoinConversation(int conversationId)
            => Groups.AddToGroupAsync(Context.ConnectionId, $"conv-{conversationId}");

        public Task LeaveConversation(int conversationId)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conv-{conversationId}");
    }
}
