using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }

        // Navigation Property
        public ICollection<Jobs> Jobs { get; set; } = new List<Jobs>();
    }

}
