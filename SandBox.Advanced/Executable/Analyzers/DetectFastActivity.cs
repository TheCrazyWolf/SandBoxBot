using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectFastActivity : SandBoxHelpers, IExecutable<bool>
{
    private bool _isOverride;
    private int _countTotalLastMessages;
    private const int ConstMaxActivityPerMinute = 10;

    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);

        AccountDb = Repository.Accounts.GetById(Update.Message.From.Id).Result;
        _countTotalLastMessages = Repository.Events
            .GetCountEventsFromIdAccount(Update.Message.From.Id,
                Update.Message.Chat.Id,
                DateTime.Now.AddMinutes(-1), DateTime.Now).Result;

        _isOverride = CanBeOverrideRestriction(Update.Message.From.Id, Update.Message.Chat.Id).Result;

        if (_isOverride) return Task.FromResult(true);

        if (_countTotalLastMessages <= ConstMaxActivityPerMinute)
            return Task.FromResult(true);

        NotifyManagers();
        return Task.FromResult(true);
    }

    private void NotifyManagers()
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
    }

    private string BuildNotifyMessage()
    {
        return
            $"\ud83d\udc7e Пользователь {Update.Message?.From?.Id} ({Update.Message?.From?.Username}) слишком часто пишет сообщения в чат. Подумайте об этом";
    }

    private IReadOnlyCollection<IReadOnlyCollection<InlineKeyboardButton>> GenerateKeyboardForNotify()
    {
        return new List<List<InlineKeyboardButton>>
        {
            new()
            {
                InlineKeyboardButton.WithCallbackData("\ud83e\ude93 Забанить юзера",
                    $"ban {Update.Message?.From?.Id} {Update.Message?.Chat.Id}")
            },
            
        };
    }
}