using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Services.Text;
using SandBox.Models.Events;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectBlackWords : SandBoxHelpers, IExecutable<bool>
{
    private bool _toDelete;
    private bool _isOverride;
    private string _blackWords = string.Empty;
    private EventContent _eventContent = new();

    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);

        AccountDb = Repository.Accounts.GetById(Update.Message.From.Id).Result;
        _toDelete = IsContainsBlackWord(Update.Message.Text);
        _isOverride = CanBeOverrideRestriction(idTelegram: Update.Message.From.Id, idChat: Update.Message.Chat.Id).Result;
        _eventContent = GenerateEvent(chatId:Update.Message.Chat.Id, content: Update.Message.Text ?? string.Empty, idTelegram: Update.Message.From.Id);
        Repository.Contents.Add(_eventContent);

        if (!_eventContent.IsSpam) return Task.FromResult(false);

        DeleteThisMessage(chatId:Update.Message.Chat.Id, messageId: Update.Message.MessageId);
        NotifyManagers();

        return Task.FromResult(true);
    }


#pragma warning disable CS8602 // Dereference of a possibly null reference.
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
#pragma warning restore CS8602 // Dereference of a possibly null reference.

    private bool GetSolutionIsSpam()
    {
        if (_isOverride)
            return !_isOverride;

        return _toDelete;
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

    private bool IsContainsBlackWord(string? message)
    {
        foreach (var word in TextTreatment.GetArrayWordsTreatmentMessage(message)
                     .Where(word => Repository.BlackWords
                         .Exists(word).Result))
        {
            _toDelete = true;
            _blackWords += $"{word} ";
        }

        return _toDelete;
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
            $"следующем содержанием: \n\n{Update.Message?.Text} \n\nЗапрещенные слова: {_blackWords} \n\n";
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