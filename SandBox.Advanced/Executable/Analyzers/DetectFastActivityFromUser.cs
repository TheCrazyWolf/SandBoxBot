using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using SandBox.Advanced.Utils.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectFastActivityFromUser(SandBoxRepository repository, ITelegramBotClient botClient, long idChat) : IAnalyzer
{
    private const int ConstMaxActivityPerMinute = 10;

    public void Execute(Message message)
    {
        if (message.From is null || message.Chat.Id != idChat)
            return;

        var account = repository.Accounts.GetByIdAsync(message.From.Id).Result;
        var totalMessage = repository.Events
            .GetCountEventsFromIdAccount(message.From.Id,
                message.Chat.Id,
                DateTime.Now.AddMinutes(-1), DateTime.Now).Result;

        if (account is null || totalMessage is 0)
            return;

        if (account.IsTrustedProfile() || botClient.IsUserAdminInChat(userId: message.From.Id, chatId: message.Chat.Id))
            return;
        
        if(totalMessage <= ConstMaxActivityPerMinute)
            return;
        
        NotifyManagers(message, GenerateKeyboardForNotify(message));
        
        return;
    }
    
    private void NotifyManagers(Message originalMessage, IList<IList<InlineKeyboardButton>> keyboardButtons)
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

    private string BuildNotifyMessage(Message originalMessage)
    {
        return
            $"\ud83d\udc7e Пользователь # {originalMessage.From?.Id} ({originalMessage.From?.Username}) слишком часто пишет сообщения в чат" +
            $" # {originalMessage.Chat.Id} - ({originalMessage.Chat.Title ?? originalMessage.Chat.FirstName}). Подумайте об этом";
    }

    private IList<IList<InlineKeyboardButton>> GenerateKeyboardForNotify(Message originalMessage)
    {
        return new List<IList<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData("\ud83e\ude93 Забанить юзера",
                    $"ban {originalMessage.From?.Id} {originalMessage.Chat.Id}")
            },
        };
    }
}