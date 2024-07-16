using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Models.Events;
using SandBox.Models.Telegram;
using SandBox_Advanced;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Services.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectSpamMl() : SandBoxHelpers, IExecutable<bool>
{
    private bool _toDelete;
    private bool _isOverride;
    private EventContent _eventContent = new();
    private float _score = 0;

    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);

        AccountDb = Repository.Accounts.GetById(Update.Message.From.Id).Result;
        _toDelete = IsSpamPredict(Update.Message.Text);
        _isOverride = CanBeOverrideRestriction(Update.Message.From.Id, Update.Message.Chat.Id).Result;
        _eventContent = GenerateEvent();
        Repository.Contents.Add(_eventContent);

        if (!_eventContent.IsSpam) return Task.FromResult(false);

        DeleteThisMessage();
        NotifyManagers();

        return Task.FromResult(true);
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
    private EventContent GenerateEvent()
    {
        return new EventContent
        {
            IsSpam = GetSolutionIsSpam(),
            ChatId = Update.Message.Chat.Id,
            DateTime = DateTime.Now,
            Content = Update.Message.Text?.ToLower() ?? string.Empty,
            IdTelegram = Update.Message.From.Id
        };
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

    private bool GetSolutionIsSpam()
    {
        if (_isOverride)
            return !_isOverride;

        return _toDelete;
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
    private Task DeleteThisMessage()
    {
        BotClient.DeleteMessageAsync(chatId: Update.Message.Chat.Id,
            messageId: Update.Message.MessageId);
        return Task.CompletedTask;
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

    private bool IsSpamPredict(string? message)
    {
        var result = MlPredictor.IsSpamPredict(message);
        _score = result.Item2;
        return result.Item1;
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
            $"\ud83d\udc7e Удалено сообщение от пользователя {Update.Message?.From?.Id} (@{Update.Message?.From?.Username}) со " +
            $"следующем содержанием: \n\n{Update.Message?.Text} \n\nℹ️ Это сообщение удалено по решению модели машинного. Вероятность спама составила {_score}%" +
            $"\n\nЕсли эта оказалось ошибкой, укажите на это. Эти данные будут использованы для обучения моделей машинного обучения";
    }

    private IReadOnlyCollection<IReadOnlyCollection<InlineKeyboardButton>> GenerateKeyboardForNotify()
    {
        return new List<List<InlineKeyboardButton>>
        {
            new()
            {
                InlineKeyboardButton.WithCallbackData("\ud83d\udd39 Восстановить",
                    $"spamrestore {_eventContent.Id}"),
                InlineKeyboardButton.WithCallbackData("\ud83e\ude93 Забанить юзера",
                    $"spamban {_eventContent.Id}")
            },

            new()
            {
                InlineKeyboardButton.WithCallbackData("\u267b\ufe0f Это не спам",
                    $"spamnospam {_eventContent.Id}")
            },
        };
    }
}