namespace Entities.Models
{
    public class Message
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }
        public Conversation? Conversation { get; set; }

        public int SenderUserId { get; set; }
        public User? Sender { get; set; }

        public string Text { get; set; } = string.Empty;      
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    }
}
