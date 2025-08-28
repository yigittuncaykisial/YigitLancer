using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Repositories;
using Entities.Models;
using System;
using YigitLancer.Hubs;
using Microsoft.EntityFrameworkCore;

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
            if (string.IsNullOrWhiteSpace(text)) return BadRequest("Text empty");

            var userIdStr = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdStr, out var senderUserId)) return Unauthorized();

            // 1) Kaydet
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

            var conv = await _ctx.Conversations
                .Where(c => c.Id == id)
                .Select(c => new { c.BuyerUserId, c.FreelancerUserId })
                .SingleOrDefaultAsync();
            if (conv is null) return NotFound("Conversation not found.");

            var recipientUserId = (senderUserId == conv.BuyerUserId)
                                    ? conv.FreelancerUserId
                                    : conv.BuyerUserId;

            // YAYIN: hem karşıya hem gönderene (iki tarayıcı açık olabilir)
            await _hub.Clients
                      .Groups($"u:{recipientUserId}", $"u:{senderUserId}") // ← iki tarayıcı için de
                      .SendAsync("NewMessage", new { conversationId = id, messageId = msg.Id });

            // Gönderen isterse optimistic UI kullanır, o yüzden 200 yeter
            if (Request.Headers["X-Requested-With"] == "fetch") return Ok(new { ok = true });
            return RedirectToAction("Conversation", new { id });
        }
        [HttpGet]
        public async Task<IActionResult> GetMessage([FromQuery] int id) // messageId
        {
            var m = await _ctx.Messages
                .Where(x => x.Id == id)
                .Select(x => new {
                    x.Id,
                    x.ConversationId,
                    x.Text,
                    x.SenderUserId,
                    x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (m is null) return NotFound();
            return Json(m);
        }
    }

   
}
