using System.ComponentModel.DataAnnotations.Schema;
using Raven.Storage.Models.Chats;
using Raven.Storage.Models.Common;
using Raven.Storage.Models.Members;

namespace Raven.Storage.Models.Messages;

public class MessageLog : Entity
{
    public long ChatId { get; set; }
    [ForeignKey("ChatId")] public ChatConfig Chat { get; set; } = default!;
    public DateTime DateTime { get; set; }
    public long TelegramId { get; set; }
    [ForeignKey("TelegramId")] public Account Account { get; set; } = default!;
    public long MessageId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsSpam { get; set; }
    public bool IsRestored { get; set; }
}