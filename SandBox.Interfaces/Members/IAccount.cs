namespace SandBox.Interfaces.Members;

public interface IAccount 
{
    public long IdTelegram { get; set; }
    public string? UserName { get; set; } 
    public string FirstName { get; set; } 
    public string? LastName { get; set; }
    public bool IsGlobalApproved { get; set; }
    public bool IsGlobalRestricted { get; set; }
    public bool IsManagerThisBot { get; set; }
}