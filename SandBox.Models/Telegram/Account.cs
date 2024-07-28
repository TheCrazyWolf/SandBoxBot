using System.ComponentModel.DataAnnotations;

namespace SandBox.Models.Telegram;

public class Account 
{
    [Key] public long IdTelegram { get; set; }
    public string? UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public bool IsGlobalApproved { get; set; }
    public bool IsGlobalRestricted { get; set; }
    public bool IsManagerThisBot { get; set; }
    
}