using SandBox.Interfaces.Common;

namespace SandBox.Interfaces.EventMessages;

public interface IEventMessage : IEvent
{
    public string Content { get; set; }
    public bool IsSpam { get; set; }
    public bool IsRestored { get; set; }
}