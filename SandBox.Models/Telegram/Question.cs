using SandBox.Models.Common;

namespace SandBox.Models.Telegram;

public class Question : Entity
{
    public string Quest { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}