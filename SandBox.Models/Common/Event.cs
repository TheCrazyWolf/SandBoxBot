namespace SandBox.Models.Common;

public class Event : Entity
{
    public long ChatId { get; set; }
    public DateTime DateTime { get; set; }
    public long AccountIdTelegram { get; set; }
}