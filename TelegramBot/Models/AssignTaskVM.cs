using TelegramBot.Entities;

namespace TelegramBot.Models
{
    public class AssignTaskVM
    {
        public IEnumerable<TaskDB> Tasks { get; set; }

       public IEnumerable<MembersDB> Members { get; set; }
    }
}
