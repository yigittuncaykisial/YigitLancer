using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Repositories.Contracts;

namespace Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly RepositoryContext _ctx;
        public ChatRepository(RepositoryContext ctx) => _ctx = ctx;

        public Conversation GetOrCreateConversation(int jobId, int buyerUserId, int freelancerUserId)
        {
            var conv = _ctx.Conversations
                .FirstOrDefault(c => c.JobId == jobId && c.BuyerUserId == buyerUserId && c.FreelancerUserId == freelancerUserId);
            if (conv != null) return conv;

            conv = new Conversation { JobId = jobId, BuyerUserId = buyerUserId, FreelancerUserId = freelancerUserId };
            _ctx.Conversations.Add(conv);
            _ctx.SaveChanges();
            return conv;
        }

        public Conversation GetConversation(int id) =>
            _ctx.Conversations
                .Include(c => c.Job)
                .Include(c => c.Messages).ThenInclude(m => m.Sender)
                .FirstOrDefault(c => c.Id == id);

        public List<Conversation> GetConversationsForUser(int userId) =>
            _ctx.Conversations
                .Include(c => c.Job)
                .Where(c => c.BuyerUserId == userId || c.FreelancerUserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

        public List<Message> GetMessages(int conversationId) =>
            _ctx.Messages
                .Include(m => m.Sender)
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .ToList();

        public void AddMessage(Message msg)
        {
            _ctx.Messages.Add(msg);
            _ctx.SaveChanges();
        }
    }
}
