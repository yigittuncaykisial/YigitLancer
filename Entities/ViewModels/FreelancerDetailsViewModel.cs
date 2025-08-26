using Entities.Models;
using System.Collections.Generic;

namespace Entities.ViewModels
{
    public class FreelancerDetailsViewModel
    {
        public User Freelancer { get; set; }                // Freelancer bilgileri
        public IEnumerable<Jobs> Jobs { get; set; }          // Freelancer’ın işleri
        public IEnumerable<Review> Reviews { get; set; }    // Freelancer’a verilen yorumlar
    }
}
