using SandBoxBot.Database.Repository;

namespace SandBoxBot.Database;

public class SandBoxRepository(SandBoxContext ef)
{
    public BlackWordRepository Words { get; set; } = new (ef);
    public SentencesRepository Incidents { get; set; } = new(ef);
    public AccountRepository Accounts { get; set; } = new(ef);
}