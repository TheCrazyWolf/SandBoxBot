using Telegram.Bot;

namespace SandBox.Advanced.Executable.Common;

public class SandBoxHelpers : EventSandBoxBase
{
    // ТОЛЬКО ДЛЯ СПАМА И ДРУГОГО АГАДИЗА
    protected Task<bool> CanBeOverrideRestriction(long idTelegram, long idChat)
    {
        if (AccountDb is null)
            return Task.FromResult(false);
        
        if (AccountDb.IsManagerThisBot)
            return Task.FromResult(AccountDb.IsManagerThisBot);

        if(AccountDb.IsNeedToVerifyByCaptcha)
            return Task.FromResult(!AccountDb.IsNeedToVerifyByCaptcha);
        
        if (AccountDb.IsAprroved) // Прошедший капчу
            return Task.FromResult(AccountDb.IsAprroved);

        if (AccountDb.IsSpamer)
            return Task.FromResult(!AccountDb.IsSpamer);

        // Доверенный профиль, вероятность того что профиль на забанят через 4 дня после спама минимальная ?
        if ((DateTime.Now.Date - AccountDb.DateTimeJoined.Date).TotalDays >= 4)
            return Task.FromResult(true);

        // проверка на админа в беседе
        var chatMembersAdmins = TryGetAdminChatUser(idTelegram, idChat).Result;

        if (!chatMembersAdmins) return Task.FromResult(false);
        
        AccountDb.IsAprroved = true;
        Repository.Accounts.Update(AccountDb);
        return Task.FromResult(true);
    }

    // ИМЕЕТ ЛИ ПРАВА АДМИНА В ПРИЦНИПЕы
    protected Task<bool> IfThisUserIsManager(long idTelegram, long idChat)
    {
        if (AccountDb is null)
            return Task.FromResult(false);

        if (AccountDb.IsManagerThisBot)
            return Task.FromResult(AccountDb.IsManagerThisBot);

        // проверка на админа в беседе
        var chatMembersAdmins = TryGetAdminChatUser(idTelegram, idChat).Result;

        if (!chatMembersAdmins) return Task.FromResult(false);
        
        AccountDb.IsAprroved = true;
        Repository.Accounts.Update(AccountDb);
        return Task.FromResult(true);
    }

    private Task<bool> TryGetAdminChatUser(long idTelegram, long idChat)
    {
        try
        {
            var chatMembersAdmins = BotClient.GetChatAdministratorsAsync(idChat).Result;

            if (chatMembersAdmins.Any(x => x.User.Id == idTelegram)) return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(false);
    }
    
}