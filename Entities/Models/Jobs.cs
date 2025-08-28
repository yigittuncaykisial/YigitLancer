using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Jobs
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public string JobDescription { get; set; }
        public string JobImg { get; set; }

        public int JobPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key: Hangi kullanıcı ekledi
        public int UserId { get; set; }
        public User User { get; set; }

        // İşin satın alınıp alınmadığı bilgisi
        public bool IsPurchased { get; set; } = false;

        // İşi satın alan kullanıcı (Opsiyonel)
        public int? PurchasedByUserId { get; set; }
        public User? PurchasedByUser { get; set; }

        // Foreign Key: Hangi kategoriye ait
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }

}
