using Entities.Models;
using System.Collections.Generic;

namespace Services.Contracts
{
    public interface IChatService
    {
        List<Conversation> GetConversationsForUser(int userId);
        Conversation? GetConversation(int conversationId, int currentUserId);
        List<Message> GetMessages(int conversationId);
        Conversation GetOrCreateConversation(int buyerUserId, int freelancerUserId, int? jobId);
        Message SendMessage(int conversationId, int senderUserId, string text);
        List<User> GetAllUsersExcept(int currentUserId);

        // Okundu & bildirim sayıları:
        void MarkMessagesRead(int conversationId, int userId);
        int GetUnreadCountForConversation(int conversationId, int userId);
        int GetUnreadConversationCount(int userId); // navbar için "okunmamış sohbet" adedi
        int GetTotalUnreadCount(int userId);        // istersen "toplam okunmamış mesaj" adedi
        bool IsLastMessageRead(int conversationId, int userId); // tek/çift tik için
    }
}
