using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SandBoxBot.Models;

public class Account
{
    [Key] public long IdAccountTelegram { get; set; }
    public string? UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public long ChatId { get; set; }
    public DateTime DateTimeJoined { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsAdmin { get; set; }
}