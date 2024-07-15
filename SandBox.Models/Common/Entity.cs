using System.ComponentModel.DataAnnotations;

namespace SandBox.Models.Common;

public class Entity
{
    [Key]
    public long Id { get; set; }
}