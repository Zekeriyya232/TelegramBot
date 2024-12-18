namespace TelegramBot.Entities
{
    public class TaskMember
    {

        public int TaskId { get; set; }
        public TaskDB Task { get; set; }

        public int MemberId { get; set; }
        public MembersDB Member { get; set; }

        public int progressPercent { get; set; } = 0;

        public string progress { get; set; } = "Başlanmadı";

        //task ilerlemesini kaydeden bir durum  %30 mesela

        //task durumunu kaydeden bir durum Başlandı , tıkandı bla bla
    }
}
