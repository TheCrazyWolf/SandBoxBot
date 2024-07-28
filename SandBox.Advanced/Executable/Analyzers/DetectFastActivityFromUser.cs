using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectFastActivityFromUser(SandBoxRepository repository, ITelegramBotClient botClient) : IAnalyzer
{
    private const int ConstMaxActivityPerMinute = 10;

    public async void Execute(Message message)
    {
        if (message.From is null)
            return;

        var account = await repository.Accounts.GetByIdAsync(message.From.Id);
        var member = await repository.MembersInChat.GetByIdAsync(idChat: message.Chat.Id, idTelegram: message.From.Id);
        
        var totalMessage = repository.Events
            .GetCountEventsFromIdAccountAsync(message.From.Id,
                message.Chat.Id,
                DateTime.Now.AddMinutes(-1), DateTime.Now).Result;

        if (account is null || member is null|| totalMessage is 0)
            return;

        if (account.IsTrustedProfile() || member.IsTrustedMember())
            return;
        
        if(totalMessage <= ConstMaxActivityPerMinute)
            return;
        
        TryNotifyManagers(message, GenerateKeyboardForNotify(message));
    }
    
    private void TryNotifyManagers(Message originalMessage, IList<IList<InlineKeyboardButton>> keyboardButtons)
    {
        var memberAdminThisChat = repository.MembersInChat.GetAdminsFromChat(originalMessage.Chat.Id).Result
            .Select(x => Convert.ToInt64(x.IdTelegram));

        var memberManagers = repository.Accounts.GetManagersAsync().Result
            .Select(x => x.IdTelegram);

        var combinedMembers = memberAdminThisChat.Union(memberManagers).ToList();
        
        foreach (var id in combinedMembers)
        {
            try
            {
                var message = BuildNotifyMessage(originalMessage);

                botClient.SendTextMessageAsync(chatId: id,
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
                InlineKeyboardButton.WithCallbackData("\ud83e\ude93 [ЧАТ] Забанить юзера",
                    $"ban {originalMessage.Chat.Id} {originalMessage.From?.Id}")
            },
        };
    }
}