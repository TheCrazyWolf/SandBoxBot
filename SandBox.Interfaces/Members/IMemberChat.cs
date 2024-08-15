using SandBox.Interfaces.Common;

namespace SandBox.Interfaces.Members;

public interface IMemberChat : IEntity
{
    public long? IdTelegram { get; set; }
    public long? IdChat { get; set; }
    public DateTime DateTimeJoined { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsRestricted { get; set; }
    public bool IsApproved { get; set; }
    public bool IsAdmin { get; set; }
}