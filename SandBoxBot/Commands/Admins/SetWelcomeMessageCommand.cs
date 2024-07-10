using SandBoxBot.Commands.Base;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace SandBoxBot.Commands.Admins;

public class SetWelcomeMessageCommand(ITelegramBotClient botClient, SandBoxRepository repository)
    : BlackBase(botClient, repository), ICommand
{
    public async Task Execute(Message message, CancellationToken cancellationToken)
    {
        var words = message.Text?.Split(' ').ToArray().Skip(1).ToArray();

        if (words == null || message.From is null)
            return;

        if (message.From != null && !await ValidateAdmin(message.From!.Id, message.Chat.Id))
            return;

        string sentence = words.Aggregate(string.Empty, (current, word) => current + $"{word} ");
        
        if(sentence.ToLower() == "clear ")
            sentence = string.Empty;

        await File.WriteAllTextAsync("Welcome.txt", sentence, cancellationToken);

        await BotClient.SendTextMessageAsync(message.Chat.Id, 
            $"\u2705 Команда выполнена" +
            $"\n\nУстановлено приветствие: {sentence}",
            cancellationToken: cancellationToken);
    }
}