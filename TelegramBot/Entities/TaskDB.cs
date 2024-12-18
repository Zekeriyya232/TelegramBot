using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.Entities
{
    [Table("Tasks")]
    public class TaskDB
    {
        public int Id { get; set; }

        public string taskName { get; set; }

        public string taskDescription { get; set; }

        [DataType(DataType.Date)]
        public DateTime startingTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime endingTime { get; set;}

        [DataType(DataType.Date)]
        public DateTime creationTime { get; set; } = DateTime.Now;

        public ICollection<TaskMember> TaskMembers { get; set; }

        public string taskGiver {  get; set; }

        //task veren kişinin adı soyadı gelebilir
    }
}
