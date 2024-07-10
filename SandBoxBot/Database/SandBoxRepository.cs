using SandBoxBot.Database.Repository;

namespace SandBoxBot.Database;

public class SandBoxRepository(SandBoxContext ef)
{
    public BlackWordRepository Words { get; set; } = new (ef);
    public AdminRepository Admins { get; set; } = new(ef);
    public SentencesRepository Sentences { get; set; } = new(ef);
}