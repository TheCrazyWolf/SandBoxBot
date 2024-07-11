using SandBoxBot.Commands.Base;
using SandBoxBot.Commands.Base.Interfaces;
using SandBoxBot.Commands.Base.Messages;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackDeleteCommand(ITelegramBotClient botClient, SandBoxRepository repository, Message? message) 
    : EventMessageCommand(botClient, repository, message), ICommand
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        if (Message?.From is null)
            return;
        
        var word = Message.Text?.Split(' ');

        if (word == null || word.Length < 2)
            return; 
                
        if (!await ValidateAdmin(Message.From.Id, Message.Chat.Id))
            return;
        
        await Repository.Words.Delete(word[1]);
            
        await BotClient.SendTextMessageAsync(Message.Chat.Id, "\u2705 Команда выполнена",
            disableNotification: true,
            cancellationToken: cancellationToken);
    }
}