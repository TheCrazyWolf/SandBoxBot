using System.ComponentModel.DataAnnotations.Schema;
using Raven.Storage.Models.Chats;
using Raven.Storage.Models.Common;

namespace Raven.Storage.Models.FAQ;

public class Question : Entity
{
    public string Quest { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public long ChatId { get; set; }
    [ForeignKey("ChatId")] public ChatConfig Chat { get; set; } = default!;
}