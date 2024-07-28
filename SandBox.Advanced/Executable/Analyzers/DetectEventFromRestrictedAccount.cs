using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectEventFromRestrictedAccount(SandBoxRepository repository, ITelegramBotClient botClient) : IAnalyzer
{
    public async void Execute(Message message)
    {
        if (message.From is null)
            return;

        var account = await repository.Accounts.GetByIdAsync(message.From.Id);
        var memberInChat = await repository.MembersInChat.GetByIdAsync(idChat: message.Chat.Id, idTelegram: message.From.Id);

        if (message.Chat.Type is not ChatType.Private) return;
        
        if (account != null && memberInChat != null && (account.IsGlobalRestricted || memberInChat.IsRestricted))
        {
            try
            {
                await botClient.DeleteMessageAsync(chatId: message.Chat.Id,
                    messageId: message.MessageId);
                TrySendNotifyMessage(message.Chat.Id);
            }
            catch
            {
                // ingore
            }
        }
    }

    private string BuildNotifyMessage()
    {
        return
            $"\ud83d\ude22 Очень жаль, что пришлось столкнутся с ограничениями. Доступ во все беседы, где используется этот анти-спам вам запрещен";
    }

    private async void TrySendNotifyMessage(long chatId)
    {
        try
        {
            var notifyMessage = BuildNotifyMessage();

            await botClient.SendTextMessageAsync(chatId: chatId,
                text: notifyMessage,
                disableNotification: true);
        }
        catch 
        {
            // ingore
        }
    }
}