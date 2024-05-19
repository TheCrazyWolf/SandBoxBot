using System.ComponentModel.DataAnnotations;

namespace SandBoxBot.Models;

public class Admin
{
    [Key]
    public long Id { get; set; }
    public long IdTelegram { get; set; }
    public string Description { get; set; }
}