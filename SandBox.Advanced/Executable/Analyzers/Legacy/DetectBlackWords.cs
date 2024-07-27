using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using SandBox.Advanced.Utils.Telegram;
using SandBox.Models.Events;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers.DeleteableMessages;

public class DetectBlackWords(SandBoxRepository repository, ITelegramBotClient botClient,
    long idChat) : IAnalyzer
{
    public void Execute(Message message)
    {
        if (message.From is null || message.Chat.Id != idChat)
            return;

        var account = repository.Accounts.GetByIdAsync(message.From.Id).Result;

        if (account is null || string.IsNullOrEmpty(message.Text))
            return;

        var blockedWords = IsContainsBlackWord(message.Text);
        var isToBlock = !string.IsNullOrEmpty(blockedWords);
        var @event = repository.Contents.GetByContent(message.Text, message.From.Id,
            message.Chat.Id, message.MessageId).Result ?? message.GenerateEventFromContent(isToBlock);
        
        if (isToBlock)
            @event.IsSpam = isToBlock;
        
        if (account.IsTrustedProfile() || botClient.IsUserAdminInChat(userId: message.From.Id,
                chatId: message.Chat.Id))
        {
            // выдать trusted??
            @event.IsSpam = false;
        }
        
        repository.Contents.UpdateAsync(@event);
        
        if (!@event.IsSpam) return;

        botClient.DeleteMessageAsync(chatId: message.Chat.Id, messageId: message.MessageId);
        NotifyManagers(message, blockedWords, GenerateKeyboardForNotify(@event));

        // т.к. этот скрипт сработал как спам, даем указанием следующим стриптам не проверять
        message.Text = null;
    }

    private string IsContainsBlackWord(string? message)
    {
        return message.GetArrayWordsTreatmentMessage()
            .Where(word => repository.BlackWords.Exists(word).Result)
            .Aggregate(string.Empty, (current, word) => current + $"{word} ");
    }

    private void NotifyManagers(Message originalMessage, string blockedWords,
        IList<IList<InlineKeyboardButton>> keyboardButtons)
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
            $"\ud83d\udc7e Удалено сообщение от пользователя {message.From?.Id} (@{message.From?.Username}) в чате # {message.Chat.Id} - ({message.Chat.Title ?? message.Chat.FirstName}) со " +
            $"следующем содержанием: \n\n{message.Text} \n\nЗапрещенные слова: {blockedWords} \n\n" +
            $"ℹ️ Если эта оказалось ошибкой, укажите на это. Эти данные будут использованы для обучения моделей машинного обучения";
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