using SandBox.Advanced.Database;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

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

        // TO DO Проверка на админа в беседе

        return Task.FromResult(false);
    }
    
    protected Task<bool> IfThisUserIsManager()
    {
        if (AccountDb is null)
            return Task.FromResult(false);
        
        if (AccountDb.IsManagerThisBot)
            return Task.FromResult(AccountDb.IsManagerThisBot);

        // to do проверка на администратор ли в беседе

        return Task.FromResult(false);
    }
}