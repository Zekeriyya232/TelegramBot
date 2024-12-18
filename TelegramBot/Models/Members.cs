using System.ComponentModel.DataAnnotations;

namespace TelegramBot.Models
{
    public class Members
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        [DataType(DataType.Date)]
        public DateTime startingJob { get; set; } 

        public long? telegramId { get; set; }

        public string phone {  get; set; }

        public string email { get; set; }

        public string category { get; set; }

        //buraya pozisyounun kategorisi

        //buraya task bağlantısı  getir
    }
}
