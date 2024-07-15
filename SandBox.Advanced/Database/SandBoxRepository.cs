using SandBox.Advanced.Database.Repository;
namespace SandBox.Advanced.Database;

public class SandBoxRepository(SandBoxContext ef)
{
    public AccountsRepository Accounts = new(ef);
    public BlackWordRepository BlackWords = new(ef);
    public EventContentRepository Contents = new(ef);
    public EventsJoinedRepository Joins = new(ef);
    public CaptchaRepository Captchas = new(ef);
}