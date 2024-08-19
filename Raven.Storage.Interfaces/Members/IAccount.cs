using Raven.Storage.Interfaces.Common;

namespace Raven.Storage.Interfaces.Members;

public interface IAccount : IEntity
{
    public bool IsBot { get; set; }
    public string FirstName { get; set; } 
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? LanguageCode { get; set; }
    public bool? IsPremium { get; set; }
    public bool? AddedToAttachmentMenu { get; set; }
    public bool? CanJoinGroups { get; set; }
    public bool? CanReadAllGroupMessages { get; set; }
    public bool? SupportsInlineQueries { get; set; }
}