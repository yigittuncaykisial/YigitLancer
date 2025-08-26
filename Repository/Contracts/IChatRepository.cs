using System.Collections.Generic;
using Entities.Models;

namespace Repositories.Contracts
{
    public interface IChatRepository
    {
        Conversation GetOrCreateConversation(int jobId, int buyerUserId, int freelancerUserId);
        Conversation GetConversation(int id);
        List<Conversation> GetConversationsForUser(int userId);
        List<Message> GetMessages(int conversationId);
        void AddMessage(Message msg);
    }
}
