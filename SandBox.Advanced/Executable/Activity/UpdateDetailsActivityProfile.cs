using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Activity;

public class UpdateDetailsActivityProfile(
    ITelegramBotClient botClient,
    Update update,
    SandBoxRepository repository) : IExecutable<bool>
{
    protected ChatTg? _chatTg;
    protected Account? _accountDb;
    
    public virtual Task<bool> Execute()
    {
        if (update.Message?.From?.Id is null)
            return Task.FromResult(false);

        _chatTg = GetThisChatTelegram();

        if (_chatTg is null)
            CreateChatIfNull();
        
        _accountDb = GetThisAccountFromDb();

        if (_accountDb is not null)
        {
            UpdateDetailsAccount();
            return Task.FromResult(true);
        }

        CreateAccountIfNull();

        return Task.FromResult(true);
    }
#pragma warning disable CS8602 // Dereference of a possibly null reference.

    protected Account? GetThisAccountFromDb()
    {
        return repository.Accounts.GetById(update.Message.From.Id).Result;
    }

    protected Task CreateAccountIfNull()
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

    protected Task UpdateDetailsAccount()
    {
        _accountDb.FirstName = update.Message.From.FirstName;
        _accountDb.LastName = update.Message.From.LastName;
        _accountDb.UserName = update.Message.From.Username;
        _accountDb.LastActivity = DateTime.Now;
        repository.Accounts.Update(_accountDb);
        return Task.CompletedTask;
    }

    protected ChatTg? GetThisChatTelegram()
    {
        return repository.Chats.GetById(update.Message.Chat.Id).Result;
    }
    
#pragma warning disable CS8601 // Possible null reference assignment.
    protected Task CreateChatIfNull()
    {
        var newChat = new ChatTg()
        {
            IdChat = update.Message.Chat.Id,
            Title = update.Message.Chat.Title ?? update.Message.Chat.FirstName
        };

        repository.Chats.Add(newChat);
        return Task.CompletedTask;
    }
#pragma warning restore CS8601 // Possible null reference assignment.
    
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}