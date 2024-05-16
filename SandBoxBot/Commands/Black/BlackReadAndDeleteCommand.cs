using SandBoxBot.Commands.Base;
using SandBoxBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackReadAndDeleteCommand : ICommand
{
    private readonly List<long> _warnsId = new List<long>()
    {
        208049718
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
        
        foreach (var id in _warnsId)
        {
            await botClient.SendTextMessageAsync(id,
                $"[!] Удалено сообщение от пользователя {message.From?.Id} ({message.From?.Username}) со следующем содержанием: \n\n{message.Text}",
                cancellationToken: cancellationToken);
        }
    }
}