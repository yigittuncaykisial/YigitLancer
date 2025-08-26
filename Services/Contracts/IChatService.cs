using Entities.Models;

namespace Services.Contracts
{
    public interface IChatService
    {
        // Konuşma listeleme
        List<Conversation> GetConversationsForUser(int userId);

        // Tek konuşma + mesajlar
        Conversation? GetConversation(int conversationId, int currentUserId);
        List<Message> GetMessages(int conversationId);

        // Konuşma oluştur (varsa getir)
        Conversation GetOrCreateConversation(int buyerUserId, int freelancerUserId, int? jobId);

        // Mesaj gönder
        Message SendMessage(int conversationId, int senderUserId, string text);

        // Sohbet oluştur ekranı: tüm kullanıcıları getir (kendisi hariç)
        List<User> GetAllUsersExcept(int currentUserId);
    }
}
