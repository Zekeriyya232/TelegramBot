namespace TelegramBot.Models
{
    public class ChatMembersVM
    {
        public long Id { get; set; }
        public string? userName { get; set; }
        public long telegramId { get; set; }

        public string? firstName { get; set; }
            
        public string? lastName { get; set; }
            
    }
}
