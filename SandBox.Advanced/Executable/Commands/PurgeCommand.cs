using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class PurgeCommand(SandBoxRepository repository, ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/purge";

    public override async void Execute(Message message)
    {
        if (message.From is null || message.ReplyToMessage is null) return;

        var account = await repository.Accounts.GetByIdAsync(message.From.Id);
        var memberInChat =
            await repository.MembersInChat.GetByIdAsync(idChat: message.Chat.Id, idTelegram: message.From.Id);

        if (account is null || memberInChat is null) return;

        if (account.IsManagerThisBot || memberInChat.IsAdmin)
        {
            var messageId = await TrySendMessage(message.Chat.Id);
            
            if (messageId is null)
                return;
            
            TryRemoveMessage(message.Chat.Id, Convert.ToInt32(messageId), message.ReplyToMessage.MessageId);
        }
    }

    private async void TryRemoveMessage(long idChat, int fromId, int toId)
    {
        int current = fromId;

        while (current >= toId)
        {
            try
            {
                _ = botClient.DeleteMessageAsync(chatId: idChat,
                    messageId: current);
                current--;
                await Task.Delay(700);
            }
            catch
            {
                // ignored
            }
        }
    }

    private async Task<int?> TrySendMessage(long idChat)
    {
        try
        {
           var result = await botClient.SendTextMessageAsync(chatId: idChat,
                text: BuildMessage(),
                disableNotification: true);
           return result.MessageId;
        }
        catch
        {
            return null;
        }

        return null;
    }

    private string BuildMessage()
    {
        return
            $"\u2705 Команда выполняется" +
            $"\n\nОжидайте";
    }
}