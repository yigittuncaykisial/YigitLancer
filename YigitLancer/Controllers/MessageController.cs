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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send([FromRoute] int id, [FromForm] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest("Text empty");

            // Kimlikten göndereni al
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdStr, out var senderUserId))
                return Unauthorized();

            // 1) DB'ye kaydet
            var msg = new Message
            {
                ConversationId = id,
                SenderUserId = senderUserId,
                Text = text.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            _ctx.Messages.Add(msg);
            await _ctx.SaveChangesAsync();

            // 2) Odaya yayın
            var payload = new
            {
                id = msg.Id,
                conversationId = msg.ConversationId,
                text = msg.Text,
                senderUserId = msg.SenderUserId,
                createdAt = msg.CreatedAt.ToString("o"),
                isRead = msg.IsRead
            };

            await _hub.Clients
                      .Group(msg.ConversationId.ToString())
                      .SendAsync("ReceiveMessage", new
                      {
                          id = msg.Id,
                          conversationId = msg.ConversationId,
                          text = msg.Text,
                          senderUserId = msg.SenderUserId,
                          createdAt = msg.CreatedAt.ToString("o"),
                          isRead = msg.IsRead
                      });


            // 3) AJAX ise 200 dön; normal post ise geri dön
            if (Request.Headers["X-Requested-With"] == "fetch")
                return Ok(payload);

            return RedirectToAction("Conversation", new { id });
        }
    }

    public record CreateMessageDto(int ConversationId, int SenderUserId, string Text);
}
