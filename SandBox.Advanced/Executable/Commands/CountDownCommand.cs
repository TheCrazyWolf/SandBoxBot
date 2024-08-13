using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Executable.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class CountDownCommand(SandBoxRepository repository, ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/countdown";
    
    public override async void Execute(Message message)
    {
        if (message.From is null)
            return;
        
        var account = await repository.Accounts.GetByIdAsync(message.From.Id);
        var memberInChat = await repository.MembersInChat.GetByIdAsync(idChat: message.Chat.Id, idTelegram: message.From.Id);

        if (account is null || memberInChat is null) return;

        if (!account.IsManagerThisBot && !memberInChat.IsAdmin) return;
        
        EndPriem.ChatId = message.Chat.Id;
        EndPriem._messageCounter = await TrySendMessage(EndPriem.ChatId, "\u26a1\ufe0f Здесь будет отсчёт до конца приема");

        if (EndPriem._messageCounter is not null)
            await TryPinMessage(EndPriem._messageCounter);
    }
    
    private async Task<Message?> TrySendMessage(long idChat, string message)
    {
        try
        {
            return await botClient.SendTextMessageAsync(chatId: idChat,
                text: message,
                disableNotification: true);
        }
        catch
        {
            // ignored
        }

        return null;
    }

    private async Task TryPinMessage(Message message)
    {
        try
        {
            await botClient.PinChatMessageAsync(message.Chat.Id, message.MessageId);
        }
        catch 
        {
            // iggnored
        }
    }
    
}