using System.ComponentModel.DataAnnotations;

namespace Raven.Storage.Models.Common;

public class Entity 
{
    [Key] public long Id { get; set; }
}