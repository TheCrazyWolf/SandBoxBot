using SandBox.Interfaces.Common;

namespace SandBox.Interfaces.Captchas;

public interface ICaptcha : IEntity
{
    public long? IdTelegram { get; set; }
    public DateTime DateTimeExpired { get; set; }
    public string Content { get; set; }
    public byte AttemptsRemain { get; set; }
}