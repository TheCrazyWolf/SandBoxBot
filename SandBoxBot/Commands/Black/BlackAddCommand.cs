using SandBoxBot.Commands.Base;
using SandBoxBot.Commands.Base.Interfaces;
using SandBoxBot.Commands.Base.Messages;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackAddCommand(ITelegramBotClient botClient, SandBoxRepository repository, Message? message)
    : EventMessageCommand(botClient, repository, message), ICommand
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        if(Message is null)
            return;
        
        var words = Message.Text?.Split(' ').ToArray().Skip(1).ToArray();

        if (words == null || Message.From is null)
            return;

        if (Message.From != null && !await ValidateAdmin(Message.From.Id, Message.Chat.Id))
            return;

        string wordToBeBlocked = string.Empty;
        
        foreach (var word in words)
        {
            await Repository.Words.Add(word);
            wordToBeBlocked += $"{word}, ";
        }
        
        await BotClient.SendTextMessageAsync(Message.Chat.Id, 
            $"\u2705 Команда выполнена" +
            $"\n\nДобавлены следующие слова: {wordToBeBlocked}",
            cancellationToken: cancellationToken);
        
    }
}