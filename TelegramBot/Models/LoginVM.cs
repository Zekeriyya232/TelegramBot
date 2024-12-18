using System.ComponentModel.DataAnnotations;

namespace TelegramBot.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage ="Kullanıcı Adınızı Giriniz!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage ="Şifrenizi Giriniz!")]
        public string Password { get; set; }
    }
}
