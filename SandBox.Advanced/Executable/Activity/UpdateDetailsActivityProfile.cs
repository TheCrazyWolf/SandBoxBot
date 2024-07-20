using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
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
            UpdateDetailsAccount(Update.Message.From);
            return Task.FromResult(true);
        }

        CreateAccountIfNull(Update.Message.From);

        return Task.FromResult(true);
    }

    protected void CreateAccountIfNull(User user)
    {
        var newAccount = new Account
        {
            IdTelegram = user.Id,
            UserName = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            LastActivity = DateTime.Now,
            DateTimeJoined = DateTime.Now,
        };

        Repository.Accounts.Add(newAccount);
    }

    protected void UpdateDetailsAccount(User user)
    {
        if(AccountDb is null)
            return;
        AccountDb.FirstName = user.FirstName;
        AccountDb.LastName = user.LastName;
        AccountDb.UserName = user.Username;
        AccountDb.LastActivity = DateTime.Now;
        Repository.Accounts.Update(AccountDb);
    }

    protected ChatTg? GetThisChatTelegram(long idChat)
    {
        return Repository.Chats.GetById(idChat).Result;
    }
    
    protected void CreateChatIfNull(long idChat, string title)
    {
        var newChat = new ChatTg
        {
            IdChat = idChat,
            Title = title
        };

        Repository.Chats.Add(newChat);
    }
}