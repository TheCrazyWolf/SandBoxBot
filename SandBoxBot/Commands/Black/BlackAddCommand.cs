using SandBoxBot.Commands.Base;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackAddCommand : BlackBase, ICommand
{
    public async Task Execute(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var words = message.Text?.Split(' ').Skip(1).ToArray();

        if (words == null)
            return;

        if (message.From == null || !await Repository.Admins.IsAdmin(message.From.Id))
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "\u26a0\ufe0f Недостаточно прав",
                cancellationToken: cancellationToken);
            return;
        }

        string wordToBeBlocked = string.Empty;
        
        foreach (var word in words)
        {
            await Repository.Words.Add(word);
            wordToBeBlocked += $"{word}, ";
        }
        
        await botClient.SendTextMessageAsync(message.Chat.Id, $"\u2705 Команда выполнена\n\nДобавлены следующие слова: {wordToBeBlocked}",
            cancellationToken: cancellationToken);
        
    }

    public BlackAddCommand(ITelegramBotClient botClient, SandBoxRepository repository) : base(botClient, repository)
    {
    }
}