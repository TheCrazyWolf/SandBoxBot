using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace SandBoxBot.Models;

public class Incident
{
    [Key] public long Id { get; set; }
    public string Value { get; set; }
    public bool IsSpam { get; set; }

    public DateTime DateTime { get; set; }

    [ForeignKey("IdAccountTelegram")] public Account? Account { get; set; }
    public long? IdAccountTelegram { get; set; }
}