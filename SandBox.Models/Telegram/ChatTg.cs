using System.ComponentModel.DataAnnotations;

namespace SandBox.Models.Telegram;

public class ChatTg
{
    [Key] public long IdChat { get; set; }
    public string Title { get; set; } = string.Empty;
}