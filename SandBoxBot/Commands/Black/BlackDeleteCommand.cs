using SandBoxBot.Commands.Base;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackDeleteCommand : BlackBase, ICommand
{
    public async Task Execute(Message message, CancellationToken cancellationToken)
    {
        var word = message.Text?.Split(' ');

        if (word == null || word.Length < 2)
            return; 
                
        if (message.From != null && !await ValidateAdmin(message.From!.Id, message.Chat.Id))
            return;
        
        await Repository.Words.Delete(word[1]);
            
        await BotClient.SendTextMessageAsync(message.Chat.Id, "\u2705 Команда выполнена",
            cancellationToken: cancellationToken);
    }

    public BlackDeleteCommand(ITelegramBotClient botClient, SandBoxRepository repository) : base(botClient, repository)
    {
    }
}