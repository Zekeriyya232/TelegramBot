using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.Entities
{

    [Table("Users")]
    public class UserDB
    {
        [Key]
        public int Id {  get; set; }
        public string userName {  get; set; }

        public string userSurname { get; set; }

        public string userEmail { get; set; }

        public string password { get; set; }

        public string? phone { get; set; }

        public string? role { get; set; } = "user";

        [DataType(DataType.Date)]
        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;

        public int? memberId { get; set; }

        public MembersDB? Members { get; set; }
    }
}
