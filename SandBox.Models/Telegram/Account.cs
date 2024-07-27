using System.ComponentModel.DataAnnotations;

namespace SandBox.Models.Telegram;

public class Account 
{
    [Key] public long IdTelegram { get; set; }
    public string? UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public DateTime DateTimeJoined { get; set; }
    public DateTime LastActivity { get; set; }
    
    
    public bool IsGlobalApproved { get; set; }
    public bool IsGlobalRestricted { get; set; }
    public bool IsManagerThisBot { get; set; }
    
    public bool IsNeedToVerifyByCaptcha { get; set; }
    public bool IsSpamer { get; set; }
    public bool IsAprroved { get; set; }
}