using SandBoxBot.Commands.Base;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackAddCommand : BlackBase, ICommand
{
    public async Task Execute(Message message, CancellationToken cancellationToken)
    {
        var words = message.Text?.Split(' ').Skip(1).ToArray();

        if (words == null || message.From is null)
            return;

        if (message.From != null && !await ValidateAdmin(message.From!.Id, message.Chat.Id))
            return;

        string wordToBeBlocked = string.Empty;
        
        foreach (var word in words)
        {
            await Repository.Words.Add(word);
            wordToBeBlocked += $"{word}, ";
        }
        
        await BotClient.SendTextMessageAsync(message.Chat.Id, $"\u2705 Команда выполнена\n\nДобавлены следующие слова: {wordToBeBlocked}",
            cancellationToken: cancellationToken);
        
    }

    public BlackAddCommand(ITelegramBotClient botClient, SandBoxRepository repository) : base(botClient, repository)
    {
    }
}