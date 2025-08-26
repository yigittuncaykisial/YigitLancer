using System;

namespace Entities.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int FreelancerId { get; set; }
        public User Freelancer { get; set; }

        public int ReviewerId { get; set; }
        public User Reviewer { get; set; }

        public int JobId { get; set; }
        public Jobs Job { get; set; }

        public int Rating { get; set; } // 1-5 arası
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
