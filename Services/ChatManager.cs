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
                .OrderByDescending(c =>
                    _ctx.Messages
                        .Where(m => m.ConversationId == c.Id)
                        .Select(m => (DateTime?)m.CreatedAt)
                        .Max() ?? c.CreatedAt) // hiç mesaj yoksa CreatedAt’e düş
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

        public void MarkMessagesRead(int conversationId, int userId)
        {
            var target = _ctx.Messages
                .Where(m => m.ConversationId == conversationId
                            && m.SenderUserId != userId
                            && !m.IsRead)
                .ToList();

            if (target.Count == 0) return;

            foreach (var m in target)
                m.IsRead = true;

            _ctx.SaveChanges();
        }

        public int GetUnreadCountForConversation(int conversationId, int userId)
        {
            return _ctx.Messages.AsNoTracking().Count(m =>
                m.ConversationId == conversationId
                && m.SenderUserId != userId
                && !m.IsRead);
        }

        public int GetUnreadConversationCount(int userId)
        {
            return _ctx.Messages.AsNoTracking()
                .Where(m => (m.Conversation.BuyerUserId == userId || m.Conversation.FreelancerUserId == userId)
                            && m.SenderUserId != userId
                            && !m.IsRead)
                .Select(m => m.ConversationId)
                .Distinct()
                .Count();
        }

        public int GetTotalUnreadCount(int userId)
        {
            return _ctx.Messages.AsNoTracking()
                .Count(m => (m.Conversation.BuyerUserId == userId || m.Conversation.FreelancerUserId == userId)
                            && m.SenderUserId != userId
                            && !m.IsRead);
        }

        public bool IsLastMessageRead(int conversationId, int userId)
        {
            var lastMyMessage = _ctx.Messages.AsNoTracking()
                .Where(m => m.ConversationId == conversationId && m.SenderUserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault();
            return lastMyMessage?.IsRead ?? false;
        }





    }

}

