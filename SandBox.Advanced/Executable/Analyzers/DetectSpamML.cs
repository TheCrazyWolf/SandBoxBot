using SandBox.Advanced.Abstract;
using SandBox.Models.Events;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Services.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectSpamMl : SandBoxHelpers, IExecutable<bool>
{
    private bool _toDelete;
    private bool _isOverride;
    private EventContent _eventContent = new();
    private float _score;

    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);

        AccountDb = Repository.Accounts.GetById(Update.Message.From.Id).Result;
        _toDelete = IsSpamPredict(Update.Message.Text);
        _isOverride = CanBeOverrideRestriction(Update.Message.From.Id, Update.Message.Chat.Id).Result;
        _eventContent = GenerateEvent(chatId: Update.Message.Chat.Id,
            content: Update.Message.Text?.ToLower() ?? string.Empty,
            idTelegram: Update.Message.From.Id);
        Repository.Contents.Add(_eventContent);

        if (!_eventContent.IsSpam) return Task.FromResult(false);
        
        if (AccountDb is not null)
        {
            AccountDb.IsSpamer = true;
            Repository.Accounts.Update(AccountDb);
        }

        BotClient.DeleteMessageAsync(chatId: Update.Message.Chat.Id,
            messageId: Update.Message.MessageId);
        
        NotifyManagers();

        return Task.FromResult(true);
    }

    private EventContent GenerateEvent(long chatId, string content, long idTelegram)
    {
        return new EventContent
        {
            IsSpam = GetSolutionIsSpam(),
            ChatId = chatId,
            DateTime = DateTime.Now,
            Content = content,
            IdTelegram = idTelegram
        };
    }
    private bool GetSolutionIsSpam()
    {
        if (_isOverride)
            return !_isOverride;

        return _toDelete;
    }

    private bool IsSpamPredict(string? message)
    {
        var result = MlPredictor.IsSpamPredict(message);
        _score = result.Item2;
        return result.Item1;
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
            $"\ud83d\udc7e Удалено сообщение от пользователя {Update.Message?.From?.Id} (@{Update.Message?.From?.Username}) со " +
            $"следующем содержанием: \n\n{Update.Message?.Text} \n\nℹ️ Это сообщение удалено по решению модели машинного обучения. Вероятность спама составила {_score}%" +
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