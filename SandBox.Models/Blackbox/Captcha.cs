using System.ComponentModel.DataAnnotations.Schema;
using SandBox.Models.Common;
using SandBox.Models.Telegram;

namespace SandBox.Models.Blackbox;

public class Captcha : Entity
{
    public long? IdTelegram { get; set; }
    [ForeignKey("IdTelegram")]public Account? Account { get; set; }
    public DateTime DateTimeExpired { get; set; }
    public string Content { get; set; } = string.Empty;
    public byte AttemptsRemain { get; set; }
}