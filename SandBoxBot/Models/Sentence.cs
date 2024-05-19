using System.ComponentModel.DataAnnotations;

namespace SandBoxBot.Models;

public class Sentence
{
    [Key]
    public long Id { get; set; }
    public string Value { get; set; }
    public bool IsSpam { get; set; }
}