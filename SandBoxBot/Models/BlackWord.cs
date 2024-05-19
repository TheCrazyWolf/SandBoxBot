using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace SandBoxBot.Models;

public class BlackWord
{
    [Key]
    public long Id { get; set; }
    public string Word { get; set; }
}