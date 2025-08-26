using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Contracts;

namespace Services
{
    public class ChatManager : IChatService
    {
        private readonly RepositoryContext _ctx;
        public ChatManager(RepositoryContext ctx) { _ctx = ctx; }

        public List<Conversation> GetConversationsForUser(int userId)
        {
            return _ctx.Conversations
                .Include(c => c.Job)
                .Include(c => c.Buyer)
                .Include(c => c.Freelancer)
                .Where(c => c.BuyerUserId == userId || c.FreelancerUserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToList();
        }

        public Conversation? GetConversation(int conversationId, int currentUserId)
        {
            return _ctx.Conversations
                .Include(c => c.Job)
                .Include(c => c.Buyer)
                .Include(c => c.Freelancer)
                .FirstOrDefault(c => c.Id == conversationId
                                  && (c.BuyerUserId == currentUserId || c.FreelancerUserId == currentUserId));
        }

        public List<Message> GetMessages(int conversationId)
        {
            return _ctx.Messages
                .Include(m => m.Sender)
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .AsNoTracking()
                .ToList();
        }

        public Conversation GetOrCreateConversation(int buyerUserId, int freelancerUserId, int? jobId)
        {
            Conversation? conv;

            if (jobId.HasValue)
            {
                // Aynı job için aynı ikiliye ait mevcut sohbeti bul
                conv = _ctx.Conversations.FirstOrDefault(c =>
                    c.BuyerUserId == buyerUserId &&
                    c.FreelancerUserId == freelancerUserId &&
                    c.JobId == jobId.Value);
            }
            else
            {
                // Job'suz (genel) sohbeti bul
                conv = _ctx.Conversations.FirstOrDefault(c =>
                    c.BuyerUserId == buyerUserId &&
                    c.FreelancerUserId == freelancerUserId &&
                    c.JobId == null);
            }

            if (conv != null) return conv;

            // YENİ: jobId yoksa da sohbet oluştur (pre-sale/general chat)
            conv = new Conversation
            {
                BuyerUserId = buyerUserId,
                FreelancerUserId = freelancerUserId,
                JobId = jobId,                   // null olabilir
                CreatedAt = DateTime.UtcNow
            };

            _ctx.Conversations.Add(conv);
            _ctx.SaveChanges();
            return conv;
        }



        public Message SendMessage(int conversationId, int senderUserId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Mesaj boş olamaz.");

            var msg = new Message
            {
                ConversationId = conversationId,
                SenderUserId = senderUserId,
                Text = text.Trim(),
                CreatedAt = DateTime.UtcNow
            };
            _ctx.Messages.Add(msg);
            _ctx.SaveChanges();
            return msg;
        }

        public List<User> GetAllUsersExcept(int currentUserId)
        {
            return _ctx.Users
                .Where(u => u.UserId != currentUserId)
                .OrderBy(u => u.UserName)
                .AsNoTracking()
                .ToList();
        }
    }
}
