using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.Entities
{
    [Table("Members")]
    public class MembersDB
    {      
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

      
        [DataType(DataType.Date)]
        public DateTime startingJob { get; set; } 

        public long? telegramId { get; set; }

        public ICollection<TaskMember> TaskMembers { get; set; }

        public string phone {  get; set; }

        public string email { get; set; }

        public string category { get; set; }

        public UserDB? User { get; set; }


    }
}
