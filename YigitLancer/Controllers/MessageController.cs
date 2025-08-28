using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Repositories;
using Entities.Models;
using System;
using YigitLancer.Hubs;

namespace YigitLancer.Controllers
{
    public class MessagesController : Controller
    {
        private readonly RepositoryContext _ctx;
        private readonly IHubContext<ChatHub> _hub;

        public MessagesController(RepositoryContext ctx, IHubContext<ChatHub> hub)
        {
            _ctx = ctx;
            _hub = hub;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] CreateMessageDto dto) // ← JSON post ediyorsan [FromBody] ekle
        {
            // 1) DB'ye kaydet
            var msg = new Message
            {
                ConversationId = dto.ConversationId,
                SenderUserId = dto.SenderUserId,
                Text = dto.Text,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            _ctx.Messages.Add(msg);
            await _ctx.SaveChangesAsync();

            // 2) AYNI ODAYA YAYIN (tam burası)
            var payload = new
            {
                id = msg.Id,
                conversationId = msg.ConversationId,    // ← ekledik
                text = msg.Text,
                senderUserId = msg.SenderUserId,
                createdAt = msg.CreatedAt.ToString("o"),
                isRead = msg.IsRead                      // ← ekledik
            };

            await _hub.Clients
                      .Group(dto.ConversationId.ToString())
                      .SendAsync("ReceiveMessage", payload);

            return Ok(payload);
        }
    }

    public record CreateMessageDto(int ConversationId, int SenderUserId, string Text);
}
