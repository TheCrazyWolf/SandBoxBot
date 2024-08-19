using Raven.Storage.Interfaces.Common;
using Telegram.Bot.Types.Enums;

namespace Raven.Storage.Interfaces.Chats;

public interface IChatConfig : IEntity
{
    public string? Title { get; set; } 
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ChatType Type { get; set; }
    public float PercentageToDetectSpamFromMl { get; set; } 
    public bool AutoKickIfWillBeDetectedSpam { get; set; }
}