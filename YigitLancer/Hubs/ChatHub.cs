
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace YigitLancer.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public Task JoinRoom(string roomId) =>
            Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        public Task LeaveRoom(string roomId) =>
            Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);


        public Task JoinConversation(string conversationId) =>
     Groups.AddToGroupAsync(Context.ConnectionId, conversationId);

    }
}
