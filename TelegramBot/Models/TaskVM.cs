using System.ComponentModel.DataAnnotations;

namespace TelegramBot.Models
{
    public class TaskVM
    {

        public int? Id { get; set; }
        public string taskName { get; set; }

        public string taskDescription { get; set; }

        [DataType(DataType.Date)]
        public DateTime startingTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime endingTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime creationTime { get; set; } 

        public string taskGiver { get; set; }
    }
}
