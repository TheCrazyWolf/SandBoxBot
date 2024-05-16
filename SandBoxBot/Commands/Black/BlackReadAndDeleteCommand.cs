using SandBoxBot.Commands.Base;
using SandBoxBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackReadAndDeleteCommand : ICommand
{
    private static readonly List<long> WarnsId = new List<long>()
    {
        // TheCrazyWolf
        208049718,
        // NV
        1238285272,
    };
    
    public async Task Execute(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        if (message.Text is null)
            return ;

        var words = message.Text.Split(' ');

        bool toDelete = false;

        foreach (var word in words)
        {
            if (BlackBoxService.Instance.IfExist(word))
                toDelete = true;
        }
        
        if(!toDelete)
            return ;

        await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId, cancellationToken: cancellationToken);
        
        foreach (var id in WarnsId)
        {
            await botClient.SendTextMessageAsync(id,
                $"[!] Удалено сообщение от пользователя {message.From?.Id} ({message.From?.Username}) со следующем содержанием: \n\n{message.Text}",
                cancellationToken: cancellationToken);
        }
    }
}