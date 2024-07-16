using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Services.Text;
using SandBox.Models.Events;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectBlackWords(
    ITelegramBotClient botClient,
    Update update,
    SandBoxRepository repository) 
{
    private Account? _accountDb;
    private bool _toDelete;
    private bool _isOverride;
    private string _blackWords = string.Empty;
    private EventContent _eventContent = new ();
    
    public Task<bool> Execute()
    {
        if (update.Message?.From is null)
            return Task.FromResult(false);
        
        _accountDb = repository.Accounts.GetById(update.Message.From.Id).Result;
        _toDelete = IsContainsBlackWord(update.Message.Text);
        _isOverride = GetOverride();
        _eventContent = GenerateEvent();
        repository.Contents.Add(_eventContent);

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
            ChatId = update.Message.Chat.Id,
            DateTime = DateTime.Now,
            Content = update.Message.Text?.ToLower() ?? string.Empty,
            IdTelegram = update.Message.From.Id
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
        if (_accountDb is null)
            return false;
        
        if(_accountDb.IsManagerThisBot)
            return _accountDb.IsManagerThisBot;

        if (_accountDb.IsAprroved) // Прошедший капчу
            return _accountDb.IsAprroved;

        // Доверенный профиль, вероятность того что профиль на забанят через 4 дня после спама минимальная ?
        if ((DateTime.Now.Date - _accountDb.DateTimeJoined.Date).TotalDays >= 4)
            return true;
        
        // TO DO Проверка на админа в беседе
        
        return false;
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
    private Task DeleteThisMessage()
    {
        botClient.DeleteMessageAsync(chatId: update.Message.Chat.Id,
            messageId: update.Message.MessageId);
        return Task.CompletedTask;
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    
    private bool IsContainsBlackWord(string? message)
    {
        foreach (var word in TextTreatment.GetArrayWordsTreatmentMessage(message)
                     .Where(word => repository.BlackWords
                         .Exists(word).Result))
        {
            _toDelete = true;
            _blackWords += $"{word} ";
        }

        return _toDelete;
    }

    private Task NotifyManagers()
    {
        foreach (var id in repository.Accounts.GetManagers().Result)
        {
            var buttons = GenerateKeyboardForNotify();
            var message = BuildNotifyMessage();

            botClient.SendTextMessageAsync(chatId:id.IdTelegram,
                text: message,
                replyMarkup: new InlineKeyboardMarkup(buttons), 
                disableNotification: true);
        }
        
        return Task.CompletedTask;
    }

    private string BuildNotifyMessage()
    {
        return
            $"\ud83d\udc7e Удалено сообщение от пользователя {update.Message?.From?.Id} (@{update.Message?.From?.Username}) со " +
            $"следующем содержанием: \n\n{update.Message?.Text} \n\nЗапрещенные слова: {_blackWords} \n\n";
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