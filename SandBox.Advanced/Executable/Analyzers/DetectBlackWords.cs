using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using SandBox.Models.Events;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectBlackWords(SandBoxRepository repository, ITelegramBotClient botClient) : IAnalyzer
{
    public bool Execute(Message message)
    {
        if (message.From is null)
            return false;

        var account = repository.Accounts.GetById(message.From.Id).Result;

        if (account is null)
            return false;
        
        var blockedWords = IsContainsBlackWord(message.Text);
        var isToBlock = !string.IsNullOrEmpty(blockedWords);
        var @event = GenerateEvent(chatId: message.Chat.Id, idTelegram: message.From.Id, content:
            message.Text, isSpam:isToBlock);

        if (account.IsTrustedProfile() || botClient.IsUserAdminInChat(userId: message.From.Id,
                chatId: message.Chat.Id))
        {
            // выдать trusted??
            @event.IsSpam = false;
        }
        else
            @event.IsSpam = isToBlock;

        repository.Contents.Add(@event);

        if (!@event.IsSpam) return false;
        
        botClient.DeleteMessageAsync(chatId: message.Chat.Id, messageId: message.MessageId);
        var keyboards = GenerateKeyboardForNotify(@event);
        NotifyManagers(message, blockedWords, keyboards);
        return true;
    }
    
    private EventContent GenerateEvent(long chatId, string? content, long idTelegram, bool isSpam)
    {
        return new EventContent
        {
            IsSpam = isSpam,
            ChatId = chatId,
            DateTime = DateTime.Now,
            Content = content ?? string.Empty,
            IdTelegram = idTelegram
        };
    }
    

    private string IsContainsBlackWord(string? message)
    {
        return message.GetArrayWordsTreatmentMessage(0)
            .Where(word => repository.BlackWords.Exists(word).Result)
            .Aggregate(string.Empty, (current, word) => current + $"{word} ");
    }

    private void NotifyManagers(Message originalMessage, string blockedWords, IList<IList<InlineKeyboardButton>> keyboardButtons)
    {
        foreach (var id in repository.Accounts.GetManagers().Result)
        {
            try
            {
                var message = BuildNotifyMessage(originalMessage, blockedWords);

                botClient.SendTextMessageAsync(chatId: id.IdTelegram,
                    text: message,
                    replyMarkup: new InlineKeyboardMarkup(keyboardButtons),
                    disableNotification: true);
            }
            catch
            {
                // ignored
            }
        }
    }

    private string BuildNotifyMessage(Message message, string blockedWords)
    {
        return
            $"\ud83d\udc7e Удалено сообщение от пользователя {message.From?.Id} (@{message.From?.Username}) со " +
            $"следующем содержанием: \n\n{message?.Text} \n\nЗапрещенные слова: {blockedWords} \n\n";
    }

    private IList<IList<InlineKeyboardButton>> GenerateKeyboardForNotify(EventContent eventContent)
    {
        return new List<IList<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData("\ud83d\udd39 Восстановить",
                    $"spamrestore {eventContent.Id}"),
                InlineKeyboardButton.WithCallbackData("\ud83e\ude93 Забанить юзера",
                    $"spamban {eventContent.Id}")
            },

            new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData("\u267b\ufe0f Это не спам",
                    $"spamnospam {eventContent.Id}")
            },
        };
    }
    
}