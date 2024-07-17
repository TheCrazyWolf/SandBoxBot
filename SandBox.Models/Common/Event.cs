using System.ComponentModel.DataAnnotations.Schema;
using SandBox.Models.Telegram;

namespace SandBox.Models.Common;

public class Event : Entity
{
    public long? ChatId { get; set; }
    [ForeignKey("ChatId")] public ChatTg? Chat { get; set; }
    public DateTime DateTime { get; set; }
    public long? IdTelegram { get; set; }
    [ForeignKey("IdTelegram")] public Account? Account { get; set; }
}