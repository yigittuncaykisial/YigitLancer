using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Entities.ViewModels
{
    public class RegisterViewModel
    {
        [Required, StringLength(30)]
        public string Name { get; set; }

        [Required, StringLength(30)]
        public string Surname { get; set; }

        [Required, StringLength(20, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string UserEmail { get; set; }

        [Required, MinLength(8)]
        public string UserPassword { get; set; }

        [Range(14, 120, ErrorMessage = "Yaş 14-120 aralığında olmalı")]
        public int? Age { get; set; }

        [Required, RegularExpression("Buyer|Freelancer", ErrorMessage = "Select Buyer or Freelancer")]
        public string UserJob { get; set; }

        [StringLength(500)]
        public string UserDescription { get; set; }

        public IFormFile? ProfileImage { get; set; }

        [Required(ErrorMessage = "Captcha is required")]
        public string Captcha { get; set; }
    }
}
