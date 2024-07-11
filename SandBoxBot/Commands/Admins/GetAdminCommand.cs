using SandBoxBot.Commands.Base;
using SandBoxBot.Commands.Base.Interfaces;
using SandBoxBot.Commands.Base.Messages;
using SandBoxBot.Database;
using SandBoxBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace SandBoxBot.Commands.Admins;

public class GetAdminCommand(ITelegramBotClient botClient, SandBoxRepository repository, Message? message) 
    : EventMessageCommand(botClient, repository, message), ICommand
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        if (Message is null)
            return;
        
        var words = Message.Text?.Split(' ').Skip(1).ToArray();

        if (words == null || Message.From is null)
            return;

        if (!CanBeChecked(out string secret))
        {
            await BotClient.SendTextMessageAsync(Message.Chat.Id,
                $"\u2705 Ошибка с секретом. Посмотрите файл в наличие или его содержимое",
                cancellationToken: cancellationToken);
            return;
        }

        if (!IsExistAccount(out Account? account, Message.Chat.Id))
        {
            await BotClient.SendTextMessageAsync(Message.Chat.Id, $"\u2705 Не удалось выдать админку",
                cancellationToken: cancellationToken);
            return;
        }

        if (secret != words[0])
        {
            await BotClient.SendTextMessageAsync(Message.Chat.Id, $"\u26a0\ufe0f Не правильный секрет",
                cancellationToken: cancellationToken);
            return;
        }

        if (account is not null)
        {
            account.IsAdmin = true;
            await Repository.Accounts.Update(account);
        }

        await BotClient.SendTextMessageAsync(Message.Chat.Id,
            $"\u2705 Команда выполнена\n\nАдминка получена",
            cancellationToken: cancellationToken);
    }

    private bool CanBeChecked(out string secret)
    {
        if (!File.Exists("Secret.txt"))
        {
            secret = string.Empty;
            return false;
        }

        secret = File.ReadAllText("Secret.txt");

        return !string.IsNullOrEmpty(secret);
    }

    private bool IsExistAccount(out Account? account, long idAccount)
    {
        account = Repository.Accounts.Get(idAccount).GetAwaiter().GetResult();

        return account is not null;
    }
    
}