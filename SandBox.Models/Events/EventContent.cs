using SandBox.Models.Common;

namespace SandBox.Models.Events;

public class EventContent : Event
{
    public string Content { get; set; } = string.Empty!;
    public bool IsSpam { get; set; }
    public bool IsRestored { get; set; }
}