using System.ComponentModel.DataAnnotations.Schema;
using SandBox.Interfaces.Members;
using SandBox.Models.Chats;
using SandBox.Models.Common;

namespace SandBox.Models.Members;

public class MemberChat : Entity, IMemberChat
{
    public long? IdTelegram { get; set; }
    [ForeignKey("IdTelegram")] public Account? Account { get; set; }
    public long? IdChat { get; set; }
    [ForeignKey("IdChat")] public ChatProps? Chat { get; set; }
    public DateTime DateTimeJoined { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsRestricted { get; set; }
    public bool IsApproved { get; set; }
    public bool IsAdmin { get; set; }
}