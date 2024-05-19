using Microsoft.EntityFrameworkCore;
using SandBoxBot.Commands.Base;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackDeleteCommand : BlackBase, ICommand
{
    public async Task Execute(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var word = message.Text?.Split(' ');

        if (word == null || word.Length < 2)
            return; 
                
        if (message.From != null && IsAdmin(message.From.Id))
        {
            await Delete(word[1].ToLower());
            
            await botClient.SendTextMessageAsync(message.Chat.Id, "[!] Команда выполнена",
                cancellationToken: cancellationToken);
        }
    }
}