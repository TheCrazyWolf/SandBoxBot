using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using SandBox.Advanced.Utils.Telegram;
using SandBox.Models.Events;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectUrlsInMsgNonTrusted(SandBoxRepository repository, ITelegramBotClient botClient, long idChat) : IAnalyzer
{
    public bool Execute(Message message)
    {
        if (message.From is null || message.Chat.Id != idChat)
            return false;

        var account = repository.Accounts.GetById(message.From.Id).Result;

        if (account is null || string.IsNullOrEmpty(message.Text))
            return false;
        
        var isToBlock = message.Text.IsContaintsUrls();
        //var @event = message.GenerateEventFromContent(isToBlock);
        // Костыль для того, чтобы пройти по второму кругу одно и тоже сообщение после Машинного обучения
        var @event = repository.Contents.GetByContent(message.Text, message.From.Id, message.Chat.Id, message.MessageId).Result ?? message.GenerateEventFromContent(isToBlock);
        
        if(isToBlock)
            @event.IsSpam = isToBlock;
        
        
        if (@event.IsSpam && @event.Id is not 0)
        {
            botClient.DeleteMessageAsync(chatId: message.Chat.Id, messageId: message.MessageId);
            NotifyManagers(message, GenerateKeyboardForNotify(@event));
            return true;
        }

        if (account.IsTrustedProfile() || botClient.IsUserAdminInChat(userId: message.From.Id,
                chatId: message.Chat.Id))
        {
            // выдать trusted??
            @event.IsSpam = false;
        }
        
        if (string.IsNullOrEmpty(message.Text))
            return false;
        
        if (@event.Id is 0)
            repository.Contents.Add(@event);
        else
            repository.Contents.Update(@event);
        
        if (!@event.IsSpam) return false;
        
        message.Text = string.Empty;
        botClient.DeleteMessageAsync(chatId: message.Chat.Id, messageId: message.MessageId);
        NotifyManagers(message, GenerateKeyboardForNotify(@event));
        message.Text = string.Empty;
        return true;
    }

    private string IsContainsBlackWord(string? message)
    {
        return message.GetArrayWordsTreatmentMessage(0)
            .Where(word => repository.BlackWords.Exists(word).Result)
            .Aggregate(string.Empty, (current, word) => current + $"{word} ");
    }

    private void NotifyManagers(Message originalMessage,
        IList<IList<InlineKeyboardButton>> keyboardButtons)
    {
        foreach (var id in repository.Accounts.GetManagers().Result)
        {
            try
            {
                var message = BuildNotifyMessage(originalMessage);

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

    private string BuildNotifyMessage(Message message)
    {
        return
            $"\ud83d\udc7e Удалено сообщение от пользователя {message.From?.Id} (@{message.From?.Username}) со " +
            $"следующем содержанием: \n\n{message?.Text} \n\n\u26a0\ufe0fМы недоверяем пользователям, которые состоят в беседе недавно и еще не прошли проверку на бота, сообщениям, которые содержат ссылки на сайты и внутри телеграмма" +
            $"\n\nЕсли эта оказалось ошибкой, укажите на это. Эти данные будут использованы для обучения моделей машинного обучения";
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