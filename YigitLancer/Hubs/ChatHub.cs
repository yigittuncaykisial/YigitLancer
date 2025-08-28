using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace YigitLancer.Hubs;   // <-- web projenin namespace'ine göre düzelt

    [Authorize]
    public class ChatHub : Hub
    {
        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }

    // (İstersen) Ayrılma
    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }

public Task JoinConversation(string conversationId)
    => Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
}
    
