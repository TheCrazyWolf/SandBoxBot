using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Activity;

public class UpdateDetailsActivityProfile : IExecutable<bool>
{
    public ITelegramBotClient BotClient = default!;
    public Update Update = default!;
    public SandBoxRepository Repository = default!;
    
    protected ChatTg? ChatTg = default!;
    protected Account? AccountDb = default!;
    
    public virtual Task<bool> Execute()
    {
        if (Update.Message?.From?.Id is null)
            return Task.FromResult(false);

        ChatTg = GetThisChatTelegram();

        if (ChatTg is null)
            CreateChatIfNull();
        
        AccountDb = GetThisAccountFromDb();

        if (AccountDb is not null)
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
        return Repository.Accounts.GetById(Update.Message.From.Id).Result;
    }

    protected Task CreateAccountIfNull()
    {
        var newAccount = new Account
        {
            IdTelegram = Update.Message.From.Id,
            UserName = Update.Message.From.Username,
            FirstName = Update.Message.From.FirstName,
            LastName = Update.Message.From.LastName,
            LastActivity = DateTime.Now,
            DateTimeJoined = DateTime.Now,
        };

        Repository.Accounts.Add(newAccount);
        return Task.CompletedTask;
    }

    protected Task UpdateDetailsAccount()
    {
        AccountDb.FirstName = Update.Message.From.FirstName;
        AccountDb.LastName = Update.Message.From.LastName;
        AccountDb.UserName = Update.Message.From.Username;
        AccountDb.LastActivity = DateTime.Now;
        Repository.Accounts.Update(AccountDb);
        return Task.CompletedTask;
    }

    protected ChatTg? GetThisChatTelegram()
    {
        return Repository.Chats.GetById(Update.Message.Chat.Id).Result;
    }
    
#pragma warning disable CS8601 // Possible null reference assignment.
    protected Task CreateChatIfNull()
    {
        var newChat = new ChatTg()
        {
            IdChat = Update.Message.Chat.Id,
            Title = Update.Message.Chat.Title ?? Update.Message.Chat.FirstName
        };

        Repository.Chats.Add(newChat);
        return Task.CompletedTask;
    }
#pragma warning restore CS8601 // Possible null reference assignment.
    
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}