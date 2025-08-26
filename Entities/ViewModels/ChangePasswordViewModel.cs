using System.ComponentModel.DataAnnotations;

namespace Entities.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mevcut şifre zorunlu.")]
        public string CurrentPassword { get; set; }

        [Required, MinLength(8, ErrorMessage = "En az 8 karakter olmalı.")]
        public string NewPassword { get; set; }

        [Required, Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
    }
}
