using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Entities.ViewModels
{
    public class JobCreateViewModel
    {
        [Required, StringLength(80)]
        public string JobName { get; set; }

        [Required, StringLength(1000)]
        public string JobDescription { get; set; }

        [Required]
        [Range(typeof(int), "1", "999999999", ErrorMessage = "Fiyat pozitif olmalı")]
        public int JobPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Kategori seçin")]
        public int? CategoryId { get; set; }

        public IFormFile? JobImgFile { get; set; }
    }
}
