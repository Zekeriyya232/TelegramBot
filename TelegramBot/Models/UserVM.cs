using System.ComponentModel.DataAnnotations;

namespace TelegramBot.Models
{
    public class UserVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        
        public string? phone { get; set; }
        public string? role { get; set; }

        [DataType(DataType.Date)]
        public DateTime RegisterDate { get; set; }
    }
}
