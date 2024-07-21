using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using SandBox.Models.Events;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectSpamMl(SandBoxRepository repository, ITelegramBotClient botClient) : IAnalyzer
{
    public bool Execute(Message message)
    {
        if (message.From is null)
            return false;

        var account = repository.Accounts.GetById(message.From.Id).Result;

        if (account is null)
            return false;

        var isToBlock = message.Text.IsSpamMl();
        var @event = message.GenerateEventFromContent(isToBlock.Item1);

        if (account.IsTrustedProfile() || botClient.IsUserAdminInChat(userId: message.From.Id,
                chatId: message.Chat.Id))
        {
            // выдать trusted??
            @event.IsSpam = false;
        }

        repository.Contents.Add(@event);

        if (!@event.IsSpam)
        {
            // Так надо для такого чтобы следующий анализатор не схватил текст
            // если на этом этапе текст прошел проверку и вынесен вердикт что спам, то дальше чекать нет смысла
            message.Text = string.Empty;
            return false;
        }

        botClient.DeleteMessageAsync(chatId: message.Chat.Id,
            messageId: message.MessageId);

        NotifyManagers(message, isToBlock.Item2, GenerateKeyboardForNotify(@event));

        return true;
    }

    private void NotifyManagers(Message originalMessage, float score,
        IList<IList<InlineKeyboardButton>> keyboardButtons)
    {
        foreach (var id in repository.Accounts.GetManagers().Result)
        {
            try
            {
                var message = BuildNotifyMessage(originalMessage, score);

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

    private string BuildNotifyMessage(Message message, float score)
    {
        return
            $"\ud83d\udc7e Удалено сообщение от пользователя {message.From?.Id} (@{message.From?.Username}) со " +
            $"следующем содержанием: \n\n{message.Text} \n\nℹ️ Это сообщение удалено по решению модели машинного обучения. Вероятность спама составила {score}%" +
            $"\n\nЕсли эта оказалось ошибкой, укажите на это. Эти данные будут использованы для обучения моделей машинного обучения";
    }
    
}