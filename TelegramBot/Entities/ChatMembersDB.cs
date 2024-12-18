using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.Entities
{
    [Table("ChatMember")]
    public class ChatMembersDB
    {
        [Key]
        public long Id { get; set; }
        public string? userName { get; set; }
        public long telegramId {  get; set; }

        public string? firstName { get; set; }

        public string? lastName { get; set; }

    }
}
