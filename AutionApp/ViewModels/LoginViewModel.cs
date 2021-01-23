using System.ComponentModel.DataAnnotations;

namespace AutionApp.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Логин: ")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Пароль: ")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня?")]
        public bool RememberMe { get; set; }
    }
}