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
    private EventContent _eventContent = new ();
    
    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);
        
        AccountDb = Repository.Accounts.GetById(Update.Message.From.Id).Result;
        _toDelete = IsContainsBlackWord(Update.Message.Text);
        _isOverride = GetOverride();
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

    private bool GetOverride()
    {
        if (AccountDb is null)
            return false;
        
        if(AccountDb.IsManagerThisBot)
            return AccountDb.IsManagerThisBot;

        if (AccountDb.IsAprroved) // Прошедший капчу
            return AccountDb.IsAprroved;

        // Доверенный профиль, вероятность того что профиль на забанят через 4 дня после спама минимальная ?
        if ((DateTime.Now.Date - AccountDb.DateTimeJoined.Date).TotalDays >= 4)
            return true;
        
        // TO DO Проверка на админа в беседе
        
        return false;
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
    private Task DeleteThisMessage()
    {
        BotClient.DeleteMessageAsync(chatId: Update.Message.Chat.Id,
            messageId: Update.Message.MessageId);
        return Task.CompletedTask;
    }
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

    private Task NotifyManagers()
    {
        foreach (var id in Repository.Accounts.GetManagers().Result)
        {
            var buttons = GenerateKeyboardForNotify();
            var message = BuildNotifyMessage();

            BotClient.SendTextMessageAsync(chatId:id.IdTelegram,
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