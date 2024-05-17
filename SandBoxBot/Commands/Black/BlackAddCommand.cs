using SandBoxBot.Commands.Base;
using SandBoxBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackAddCommand : ICommand
{
    private static readonly List<long> AllowedId = new List<long>()
    {
        // TheCrazyWolf
        208049718,
        // NV
        1238285272,
        // Anastasiya
        508925377,
        // vladimir
        430154369
    };

    public async Task Execute(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var word = message.Text?.Split(' ');

        if (word == null || word.Length < 2)
            return;

        if (message.From != null && AllowedId.Contains(message.From.Id))
        {
            BlackBoxService.Instance.AddWord(word[1]);

            await botClient.SendTextMessageAsync(message.Chat.Id, "[!] Команда выполнена",
                cancellationToken: cancellationToken);
        }
    }
}