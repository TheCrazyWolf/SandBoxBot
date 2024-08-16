using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class RemoveCommand(SandBoxRepository repository, ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/remove";

    public override async void Execute(Message message)
    {

        if (message.From is null || message.ReplyToMessage is null)
        {
            return;
        }

        var account = await repository.Accounts.GetByIdAsync(message.From.Id);
        
        var memberInChat = await repository.MembersInChat.GetByIdAsync(idChat: message.Chat.Id, idTelegram: message.From.Id);

        if (account is null || memberInChat is null) return;

        if (!account.IsManagerThisBot && !memberInChat.IsAdmin) return;

        TryRemoveMessage(message.ReplyToMessage.Chat.Id, message.ReplyToMessage.MessageId);
        TryRemoveMessage(message.Chat.Id, message.MessageId);
    }

    private async void TryRemoveMessage(long idChat, int messageId)
    {
        try
        {
            await botClient.DeleteMessageAsync(chatId: idChat,
                messageId: messageId);
        }
        catch
        {
            // ignored
        }
    }
    
}