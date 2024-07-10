using Microsoft.EntityFrameworkCore;
using SandBoxBot.Database;
using SandBoxBot.Models;
using Telegram.Bot;

namespace SandBoxBot.Commands.Base;

public class BlackBase(ITelegramBotClient botClient, SandBoxRepository repository)
{
    protected ITelegramBotClient BotClient = botClient;
    protected readonly SandBoxRepository Repository = repository;
    
    protected string[] GetArrayWordsTreatmentMessage(string message)
    {
        return message.Replace('.', ' ')
            .Replace('-', ' ')
            .Replace(',', ' ')
            .Replace('!', ' ')
            .Replace("\n", " ")
            .Replace('?', ' ')
            .Replace(':', ' ')
            .Replace("  ", " ")
            .Replace(" ", " ")
            .Split(' ')
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();
    }
    
}