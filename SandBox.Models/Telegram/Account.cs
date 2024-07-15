using SandBox.Models.Common;

namespace SandBox.Models.Telegram;

public class Account : Entity
{
    public long AccountIdTelegram { get; set; }
    public string? UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public DateTime DateTimeJoined { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsSpamer { get; set; }
    public bool IsAprroved { get; set; }
    public bool IsManagerThisBot { get; set; }
}