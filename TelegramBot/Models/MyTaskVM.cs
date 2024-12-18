using System.ComponentModel.DataAnnotations;

namespace TelegramBot.Models
{
    public class MyTaskVM
    {
        public int taskId { get; set; }

        public string progress { get; set; }

        public int progressPercent { get; set; }

    }
}
