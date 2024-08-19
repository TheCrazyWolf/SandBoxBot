using System.ComponentModel.DataAnnotations.Schema;
using Raven.Storage.Models.Chats;
using Raven.Storage.Models.Common;

namespace Raven.Storage.Models.Members;

public class MemberChat : Entity
{
    public long TelegramId { get; set; }
    [ForeignKey("TelegramId")] public Account Account { get; set; } = default!;
    public long ChatId { get; set; }
    [ForeignKey("ChatId")] public ChatConfig Chat { get; set; } = default!;
    public DateTime DateTimeJoined { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsRestricted { get; set; }
    public bool IsApproved { get; set; }
    public bool IsAdmin { get; set; }
}