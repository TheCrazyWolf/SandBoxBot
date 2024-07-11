using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Base.Messages;

public class EventMessageCommand(ITelegramBotClient botClient, SandBoxRepository repository, Message? message) 
    : HelpersTelegramValidation(botClient, repository)
{
    protected readonly Message? Message = message;
}