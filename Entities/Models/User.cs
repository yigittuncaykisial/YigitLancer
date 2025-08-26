using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Fullname => Name + " " + Surname;
        public string? UserJob { get; set; } // Kullanıcının mesleği
        public bool IsAdmin { get; set; }
        public string? UserDescription { get; set; }
        public string? UserEmail { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }
        public int? Age { get; set; }
        public string? ProfileImagePath { get; set; } // Resmin server yolu veya URL


        // Navigation Property (İlişki)
        public ICollection<Jobs> Jobs { get; set; } = new List<Jobs>();             // Sahibi olduğu ilanlar
        public ICollection<Jobs> PurchasedJobs { get; set; } = new List<Jobs>();    // Satın aldığı ilanlar

    }

}
