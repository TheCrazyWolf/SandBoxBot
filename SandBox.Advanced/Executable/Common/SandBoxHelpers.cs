using Telegram.Bot;

namespace SandBox.Advanced.Executable.Common;

public class SandBoxHelpers : EventSandBoxBase
{
    protected Task<bool> CanBeOverrideRestriction(long idTelegram, long idChat)
    {
        if (AccountDb is null)
            return Task.FromResult(false);

        if (AccountDb.IsManagerThisBot)
            return Task.FromResult(AccountDb.IsManagerThisBot);

        if (AccountDb.IsAprroved) // Прошедший капчу
            return Task.FromResult(AccountDb.IsAprroved);

        // Доверенный профиль, вероятность того что профиль на забанят через 4 дня после спама минимальная ?
        if ((DateTime.Now.Date - AccountDb.DateTimeJoined.Date).TotalDays >= 4)
            return Task.FromResult(true);
        
        // проверка на админа в беседе
        var chatMembersAdmins = BotClient.GetChatAdministratorsAsync(idChat).Result;

        if (!chatMembersAdmins.Any(x => x.User.Id == idTelegram)) return Task.FromResult(false);
       
        AccountDb.IsAprroved = true;
        Repository.Accounts.Update(AccountDb);
        return Task.FromResult(true);
        
    }
    
    protected Task<bool> IfThisUserIsManager(long idTelegram, long idChat)
    {
        if (AccountDb is null)
            return Task.FromResult(false);
        
        if (AccountDb.IsManagerThisBot)
            return Task.FromResult(AccountDb.IsManagerThisBot);

        // to do проверка на администратор ли в беседе
        var chatMembersAdmins = BotClient.GetChatAdministratorsAsync(idChat).Result;

        if (!chatMembersAdmins.Any(x => x.User.Id == idTelegram)) return Task.FromResult(false);
       
        AccountDb.IsAprroved = true;
        Repository.Accounts.Update(AccountDb);
        
        return Task.FromResult(false);
    }
}