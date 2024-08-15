using System.ComponentModel.DataAnnotations;
using Telegram.Bot.Types.Enums;

namespace SandBox.Interfaces.Chats;

public interface IChat
{
    [Key] public long IdChat { get; set; }
    public string? Title { get; set; } 
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ChatType Type { get; set; }
    public float PercentageToDetectSpamFromMl { get; set; } 
    public bool AutoKickIfWillBeDetectedSpam { get; set; }
}