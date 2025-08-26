using System;

namespace Entities.Models
{
    public enum PurchaseRequestStatus { Pending = 0, Accepted = 1, Rejected = 2, Cancelled = 3 }

    public class PurchaseRequest
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int BuyerUserId { get; set; }
        public string? Note { get; set; }
        public PurchaseRequestStatus Status { get; set; } = PurchaseRequestStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DecidedAt { get; set; }

        // Navs
        public Jobs Job { get; set; }
        public User Buyer { get; set; }
    }
}
