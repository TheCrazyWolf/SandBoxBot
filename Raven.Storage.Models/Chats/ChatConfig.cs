using Raven.Storage.Models.Common;
using Telegram.Bot.Types.Enums;

namespace Raven.Storage.Models.Chats;

public class ChatConfig : Entity
{
    public string? Title { get; set; } 
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ChatType Type { get; set; }
    public float PercentageToDetectSpamFromMl { get; set; } = 0.55f;
    public bool AutoKickIfWillBeDetectedSpam { get; set; } = true;
}