using System.ComponentModel.DataAnnotations.Schema;
using Raven.Storage.Models.Chats;
using Raven.Storage.Models.Common;

namespace Raven.Storage.Models.Countdowns;

public class CountDown : Entity
{
    public long ChatId { get; set; }
    [ForeignKey("ChatId")] public ChatConfig Chat { get; set; } = default!;
    public string MessageCountDown { get; set; } = string.Empty;
    public string MessageCountDownOff { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}