using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Base;

public interface ICommand
{
    Task Execute(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);
}