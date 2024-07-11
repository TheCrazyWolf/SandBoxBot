using SandBoxBot.Database;
using Telegram.Bot;

namespace SandBoxBot.Commands.Base;

public class HelpersTelegramValidation(ITelegramBotClient botClient, SandBoxRepository repository)
{
    protected readonly ITelegramBotClient BotClient = botClient;
    protected readonly SandBoxRepository Repository = repository;

    protected async Task<bool> ValidateAdmin(long idAccount, long chatId)
    {
        if (await Repository.Accounts.IsAdmin(idAccount))
            return true;

        await BotClient.SendTextMessageAsync(chatId, "\u26a0\ufe0f Недостаточно прав", disableNotification: true);
        return false;
    }
}