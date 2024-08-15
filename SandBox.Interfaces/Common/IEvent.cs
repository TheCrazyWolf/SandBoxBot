namespace SandBox.Interfaces.Common;

public interface IEvent : IEntity
{
    public long? ChatId { get; set; }
    public DateTime DateTime { get; set; }
    public long? IdTelegram { get; set; }
    public long MessageId { get; set; }
}