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

    protected ChatTg? ChatTg;
    protected Account? AccountDb;
    
    public virtual Task<bool> Execute()
    {
        if (Update.Message?.From?.Id is null)
            return Task.FromResult(false);

        ChatTg = GetThisChatTelegram(Update.Message.Chat.Id);

        if (ChatTg is null)
            CreateChatIfNull(idChat: Update.Message.Chat.Id,
                title: Update.Message.Chat.Title ?? Update.Message.Chat.FirstName ?? string.Empty);
        
        AccountDb = Repository.Accounts.GetById(Update.Message.From.Id).Result;

        if (AccountDb is not null)
        {
            UpdateDetailsAccount();
            return Task.FromResult(true);
        }

        CreateAccountIfNull();

        return Task.FromResult(true);
    }
#pragma warning disable CS8602 // Dereference of a possibly null reference.

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

    protected ChatTg? GetThisChatTelegram(long idChat)
    {
        return Repository.Chats.GetById(idChat).Result;
    }
    
#pragma warning disable CS8601 // Possible null reference assignment.
    protected void CreateChatIfNull(long idChat, string title)
    {
        var newChat = new ChatTg
        {
            IdChat = idChat,
            Title = title
        };

        Repository.Chats.Add(newChat);
    }
#pragma warning restore CS8601 // Possible null reference assignment.
    
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}