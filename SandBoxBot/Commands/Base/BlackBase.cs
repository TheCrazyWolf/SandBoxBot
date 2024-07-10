using Microsoft.EntityFrameworkCore;
using SandBoxBot.Database;
using SandBoxBot.Models;
using Telegram.Bot;

namespace SandBoxBot.Commands.Base;

public class BlackBase(ITelegramBotClient botClient, SandBoxRepository repository)
{
    protected ITelegramBotClient BotClient = botClient;
    protected readonly SandBoxRepository Repository = repository;

    protected async Task<bool> ValidateAdmin(long idAccount, long chatId)
    {
        if (await Repository.Admins.IsAdmin(idAccount))
            return true;

        await botClient.SendTextMessageAsync(chatId, "\u26a0\ufe0f Недостаточно прав");
        return false;
    }
}