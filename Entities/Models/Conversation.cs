namespace Entities.Models
{
    public class Conversation
    {
        public int Id { get; set; }

        public int? JobId { get; set; }
        public Jobs? Job { get; set; }

        public int BuyerUserId { get; set; }
        public User? Buyer { get; set; }

        public int FreelancerUserId { get; set; }
        public User? Freelancer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
