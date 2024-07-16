using Microsoft.EntityFrameworkCore;
using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Activity;

public class UpdateDetailsActivityProfile(
    ITelegramBotClient botClient,
    Update update,
    SandBoxRepository repository) : IExecutable
{
    private ChatTg? _chatTg;
    private Account? _accountDb;
    
    public Task Execute()
    {
        if (update.Message?.From?.Id is null)
            return Task.CompletedTask;

        _accountDb = GetThisAccountFromDb();

        if (_accountDb is not null)
        {
            UpdateDetailsAccount();
            return Task.CompletedTask;
        }

        CreateAccountIfNull();

        return Task.CompletedTask;
    }
#pragma warning disable CS8602 // Dereference of a possibly null reference.

    private Account? GetThisAccountFromDb()
    {
        return repository.Accounts.GetById(update.Message.From.Id).Result;
    }

    private Task CreateAccountIfNull()
    {
        var newAccount = new Account
        {
            IdTelegram = update.Message.From.Id,
            UserName = update.Message.From.Username,
            FirstName = update.Message.From.FirstName,
            LastName = update.Message.From.LastName,
            LastActivity = DateTime.Now,
            DateTimeJoined = DateTime.Now,
        };

        repository.Accounts.Add(newAccount);
        return Task.CompletedTask;
    }

    private Task UpdateDetailsAccount()
    {
        _accountDb.FirstName = update.Message.From.FirstName;
        _accountDb.LastName = update.Message.From.LastName;
        _accountDb.UserName = update.Message.From.Username;
        _accountDb.LastActivity = DateTime.Now;
        repository.Accounts.Update(_accountDb);
        return Task.CompletedTask;
    }

    /*private ChatTg? GetThisChatTelegram()
    {
        return repository.
    }*/
    
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}