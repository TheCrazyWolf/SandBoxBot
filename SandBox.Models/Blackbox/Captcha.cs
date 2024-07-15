using SandBox.Models.Common;

namespace SandBox.Models.Blackbox;

public class Captcha : Entity
{
    public long AccountIdTelegram { get; set; }
    public DateTime DateTimeExpired { get; set; }
    public string Content { get; set; }
    public byte AttemptsRemain { get; set; }
}