using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectAntiArab() : EventSandBoxBase, IExecutable<bool>
{

    public Task<bool> Execute()
    {
        return Task.FromResult(false);
    }

    private bool GetOverride()
    {
        if (AccountDb is null)
            return false;

        if (AccountDb.IsManagerThisBot)
            return AccountDb.IsManagerThisBot;

        if (AccountDb.IsAprroved) // Прошедший капчу
            return AccountDb.IsAprroved;

        // Доверенный профиль, вероятность того что профиль на забанят через 4 дня после спама минимальная ?
        if ((DateTime.Now.Date - AccountDb.DateTimeJoined.Date).TotalDays >= 4)
            return true;

        // TO DO Проверка на админа в беседе

        return false;
    }
    
    private Task NotifyManagers()
    {
        foreach (var id in Repository.Accounts.GetManagers().Result)
        {
            var buttons = GenerateKeyboardForNotify();
            var message = BuildNotifyMessage();

            BotClient.SendTextMessageAsync(chatId: id.IdTelegram,
                text: message,
                replyMarkup: new InlineKeyboardMarkup(buttons),
                disableNotification: true);
        }

        return Task.CompletedTask;
    }

    private string BuildNotifyMessage()
    {
        return
            $"";
    }

    private IReadOnlyCollection<IReadOnlyCollection<InlineKeyboardButton>> GenerateKeyboardForNotify()
    {
        return new List<List<InlineKeyboardButton>>
        {
            new()
            {
                InlineKeyboardButton.WithCallbackData("\ud83d\udd39 Восстановить",
                    $"blackword restore "),
                InlineKeyboardButton.WithCallbackData("\ud83e\ude93 Забанить юзера",
                    $"blackword ban ")
            },

            new()
            {
                InlineKeyboardButton.WithCallbackData("\u267b\ufe0f Это не спам",
                    $"blackword nospam ")
            },
        };
    }
}