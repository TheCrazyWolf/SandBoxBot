using System.ComponentModel.DataAnnotations;
using Telegram.Bot.Types.Enums;

namespace SandBox.Models.Telegram;

public class ChatProps
{
    [Key] public long IdChat { get; set; }
    public string? Title { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ChatType Type { get; set; }
    public float PercentageToDetectSpamFromMl { get; set; } = 0.55f;
}