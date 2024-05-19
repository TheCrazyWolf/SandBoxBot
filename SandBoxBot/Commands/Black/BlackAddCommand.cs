using SandBoxBot.Commands.Base;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackAddCommand : BlackBase, ICommand
{
    public async Task Execute(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var word = message.Text?.Split(' ');

        if (word == null || word.Length < 2)
            return;

        if (message.From != null && IsAdmin(message.From.Id))
        {
            await Add(word[1].ToLower());

            await botClient.SendTextMessageAsync(message.Chat.Id, "[!] Команда выполнена",
                cancellationToken: cancellationToken);
        }
    }
}