using Microsoft.EntityFrameworkCore.Metadata;
using SandBoxBot.Commands.Base.Interfaces;
using SandBoxBot.Commands.Base.Messages;
using SandBoxBot.Configs;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Admins;

public class SetLevinshtainCommand(ITelegramBotClient botClient, SandBoxRepository repository, Message? message) 
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

        try
        {
            GlobalConfigs.IsWorkLevinshtain = Convert.ToBoolean(words[0]);
            await BotClient.SendTextMessageAsync(Message.Chat.Id, 
                $"\u2705 Команда выполнена" +
                $"\n\nУстановлено Алгоритму Левинштейна новое значение: {GlobalConfigs.IsWorkLevinshtain}",
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            await BotClient.SendTextMessageAsync(Message.Chat.Id, 
                $"\u2705 Команда выполнена" +
                $"\n\nОшибка при установке нового значения",
                cancellationToken: cancellationToken);
        }
        
    }
}