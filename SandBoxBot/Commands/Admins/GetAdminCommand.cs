using SandBoxBot.Commands.Base;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace SandBoxBot.Commands.Admins;

public class GetAdminCommand : BlackBase, ICommand
{
    public async Task Execute(Message message, CancellationToken cancellationToken)
    {
        var words = message.Text?.Split(' ').Skip(1).ToArray();

        if (words == null || message.From is null)
            return;

        if (!File.Exists("Secret.txt"))
        {
            await BotClient.SendTextMessageAsync(message.Chat.Id, $"\u2705 Секрет пустой, настройте",
                cancellationToken: cancellationToken);
            return;
        }
        
        var secret = await File.ReadAllTextAsync("Secret.txt", cancellationToken);

        if (string.IsNullOrEmpty(secret))
        {
            await BotClient.SendTextMessageAsync(message.Chat.Id, $"\u2705 Секрет пустой, настройте",
                cancellationToken: cancellationToken);
            return;
        }
        
        
        var account = await Repository.Accounts.Get(message.From.Id);
        
        if (account is null)
        {
            await BotClient.SendTextMessageAsync(message.Chat.Id, $"\u2705 Не удалось выдать админку",
                cancellationToken: cancellationToken);
            return;
        }

        if (secret != words[0])
        {
            await BotClient.SendTextMessageAsync(message.Chat.Id, $"\u26a0\ufe0f Не правильный секрет",
                cancellationToken: cancellationToken);
            return;
        }
        
        account.IsAdmin = true;
        await Repository.Accounts.Update(account);
        
        await BotClient.SendTextMessageAsync(message.Chat.Id, $"\u2705 Команда выполнена\n\nАдминка получена",
            cancellationToken: cancellationToken);
        
    }

    public GetAdminCommand(ITelegramBotClient botClient, SandBoxRepository repository) : base(botClient, repository)
    {
    }
}