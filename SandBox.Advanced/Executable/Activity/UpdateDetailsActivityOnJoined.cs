using SandBox.Models.Events;
using SandBox.Models.Telegram;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Activity;

public class UpdateDetailsActivityOnJoined: UpdateDetailsActivityProfile
{
    public override Task<bool> Execute()
    {
        if (Update.Message?.NewChatMembers is null)
            return Task.FromResult(false);

        ChatTg = GetThisChatTelegram();

        if (ChatTg is null)
            CreateChatIfNull();

        foreach (var user in Update.Message.NewChatMembers)
        {
            CreateAccount(user);
            CreateEventJoin(user);
        }
        return Task.FromResult(true);
    }

    private EventJoined CreateEventJoin(User user)
    {
        var newEvent = new EventJoined
        {
            IdTelegram = user.Id,
            ChatId = Update.Message?.Chat.Id,
            DateTime = DateTime.Now
        };

        return Repository.Joins.Add(newEvent).Result;
    }
    
    protected Task CreateAccount(User user)
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
        return Task.CompletedTask;
    }
}