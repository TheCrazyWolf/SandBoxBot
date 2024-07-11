using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Base.Callback;

public class EventCallbackQueryCommand(ITelegramBotClient botClient, SandBoxRepository repository, CallbackQuery callbackQuery) : HelpersTelegramValidation(botClient, repository)
{
    protected readonly CallbackQuery CallbackQuery = callbackQuery;
}