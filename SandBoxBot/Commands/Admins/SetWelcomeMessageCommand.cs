using SandBoxBot.Commands.Base;
using SandBoxBot.Commands.Base.Interfaces;
using SandBoxBot.Commands.Base.Messages;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace SandBoxBot.Commands.Admins;

public class SetWelcomeMessageCommand(ITelegramBotClient botClient, SandBoxRepository repository, Message? message) 
    : EventMessageCommand(botClient, repository, message), ICommand
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        if(Message is null)
            return;
        
        var words = Message.Text?.Split(' ').ToArray().Skip(1).ToArray();

        if (words == null || Message.From is null)
            return;

        if (Message.From != null && !await ValidateAdmin(Message.From!.Id, Message.Chat.Id))
            return;

        string sentence = words.Aggregate(string.Empty, (current, word) => current + $"{word} ");
        
        if(sentence.ToLower() == "clear ")
            sentence = string.Empty;

        await File.WriteAllTextAsync("Welcome.txt", sentence, cancellationToken);

        await BotClient.SendTextMessageAsync(Message.Chat.Id, 
            $"\u2705 Команда выполнена" +
            $"\n\nУстановлено приветствие: {sentence}",
            cancellationToken: cancellationToken);
    }
}