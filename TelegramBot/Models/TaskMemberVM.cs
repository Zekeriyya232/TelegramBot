namespace TelegramBot.Models
{
    public class TaskMemberVM
    {
        public IEnumerable<Members> members { get; set; }

        public IEnumerable<TaskVM> tasks { get; set; }

        public string progress { get; set; }    

        public int progressPercent { get; set; }
    }
}
