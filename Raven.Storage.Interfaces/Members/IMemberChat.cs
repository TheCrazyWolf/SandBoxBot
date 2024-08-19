using Raven.Storage.Interfaces.Common;

namespace Raven.Storage.Interfaces.Members;

public interface IMemberChat : IEntity
{
    public long TelegramId { get; set; }
    public Account Account { get; set; } 
    public long ChatId { get; set; }
    public ChatConfig Chat { get; set; }
    public DateTime DateTimeJoined { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsRestricted { get; set; }
    public bool IsApproved { get; set; }
    public bool IsAdmin { get; set; }
}