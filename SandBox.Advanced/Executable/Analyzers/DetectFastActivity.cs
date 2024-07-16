using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Services;
using SandBox.Models.Events;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectFastActivity(
    ITelegramBotClient botClient,
    Update update,
    SandBoxRepository repository) : IExecutable
{
    private Account? _accountDb;
    private bool _isOverride;
    private int _countTotalLastMessages;
    private const int ConstMaxActivityPerMinute = 10;

    public Task Execute()
    {
        if (update.Message?.From is null)
            return Task.CompletedTask;

        _accountDb = repository.Accounts.GetById(update.Message.From.Id).Result;
        _countTotalLastMessages = repository.Events
            .GetCountEventsFromIdAccount(update.Message.From.Id,
                update.Message.Chat.Id,
                DateTime.Now.AddMinutes(-1), DateTime.Now).Result;

        _isOverride = GetOverride();

        if (_isOverride) return Task.CompletedTask;

        if (_countTotalLastMessages <= ConstMaxActivityPerMinute)
            return Task.CompletedTask;
        
        NotifyManagers();
        return Task.CompletedTask;
    }

    private bool GetOverride()
    {
        if (_accountDb is null)
            return false;

        if (_accountDb.IsManagerThisBot)
            return _accountDb.IsManagerThisBot;

        if (_accountDb.IsAprroved) // Прошедший капчу
            return _accountDb.IsAprroved;

        // Доверенный профиль, вероятность того что профиль на забанят через 4 дня после спама минимальная ?
        if ((DateTime.Now.Date - _accountDb.DateTimeJoined.Date).TotalDays >= 4)
            return true;

        // TO DO Проверка на админа в беседе

        return false;
    }
    
    private Task NotifyManagers()
    {
        foreach (var id in repository.Accounts.GetManagers().Result)
        {
            var buttons = GenerateKeyboardForNotify();
            var message = BuildNotifyMessage();

            botClient.SendTextMessageAsync(chatId: id.IdTelegram,
                text: message,
                replyMarkup: new InlineKeyboardMarkup(buttons),
                disableNotification: true);
        }

        return Task.CompletedTask;
    }

    private string BuildNotifyMessage()
    {
        return
            $"\ud83d\udc7e Удалено сообщение от пользователя {update.Message?.From?.Id} ({update.Message?.From?.Username}) со " +
            $"следующем содержанием: \n\n{update.Message?.Text} \n\n";
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