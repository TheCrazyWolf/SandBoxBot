using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using SandBox.Advanced.Utils.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectFastJoins(SandBoxRepository repository, ITelegramBotClient botClient, long idChat) : IAnalyzer
{
    private const int ConstMaxActivityPerMinute = 10;

    public async void Execute(Message message)
    {
        if (message.From is null || message.Chat.Id != idChat)
            return;

        var account = repository.Accounts.GetByIdAsync(message.From.Id).Result;
        var totalJoins = await repository.Joins
            .GetCountJoinsFromChatAsync(message.Chat.Id,
                DateTime.Now.AddMinutes(-1), DateTime.Now);

        if (account is null || totalJoins is 0)
            return;

        if (account.IsTrustedProfile() || botClient.IsUserAdminInChat(userId: message.From.Id, chatId: message.Chat.Id))
            return;

        if (totalJoins <= ConstMaxActivityPerMinute)
            return;

        NotifyManagers(message, GenerateKeyboardForNotify(message));
    }

    private void NotifyManagers(Message originalMessage, IList<IList<InlineKeyboardButton>> keyboardButtons)
    {
        foreach (var id in repository.Accounts.GetManagersAsync().Result)
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
            $"\ud83d\udc7e В чате # {originalMessage.Chat.Id} - ({originalMessage.Chat.Title ?? originalMessage.Chat.FirstName}) " +
            $"происходит аномальная активность: Слишком много пользователей заходят в чат за 1 минуту, похоже на атаку ботов";
    }

    private IList<IList<InlineKeyboardButton>> GenerateKeyboardForNotify(Message originalMessage)
    {
        return new List<IList<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("\ud83e\ude93 [] Забанить юзера",
                    $"ban {originalMessage.Chat.Id} {originalMessage.From?.Id}")
            },
        };
    }
}