using SandBox.Models.Events;
using SandBox.Models.Telegram;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Utils.Telegram;

public static class UserUtils
{
    public static Account CreateAccountDb(this User user)
    {
        return new Account
        {
            IdTelegram = user.Id,
            UserName = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            LastActivity = DateTime.Now,
            DateTimeJoined = DateTime.Now,
        };
    }

    public static EventJoined CreateEventJoinFromUser(this User user, long chatId)
    {
        return new EventJoined
        {
            IdTelegram = user.Id,
            ChatId = chatId,
            DateTime = DateTime.Now
        };
    }
}