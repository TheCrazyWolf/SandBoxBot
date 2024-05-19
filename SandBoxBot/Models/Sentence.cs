using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace SandBoxBot.Models;

public class Sentence
{
    [Key]
    public long Id { get; set; }
    public string Value { get; set; }
    public bool IsSpam { get; set; }
}