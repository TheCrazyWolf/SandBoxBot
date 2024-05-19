using System.ComponentModel.DataAnnotations;

namespace SandBoxBot.Models;

public class BlackWord
{
    [Key]
    public long Id { get; set; }
    public string Word { get; set; }
}