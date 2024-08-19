using System.ComponentModel.DataAnnotations.Schema;
using Raven.Storage.Models.Common;
using Raven.Storage.Models.Members;

namespace Raven.Storage.Models.Captchas;

public class Captcha : Entity
{
    public long IdTelegram { get; set; }
    [ForeignKey("IdTelegram")] public Account Account { get; set; } = default!;
    public DateTime DateTimeExpired { get; set; }
    public string Content { get; set; } = string.Empty;
    public byte AttemptsRemain { get; set; }
}