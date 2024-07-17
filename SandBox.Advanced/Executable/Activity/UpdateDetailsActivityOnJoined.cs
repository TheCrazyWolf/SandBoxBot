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

        ChatTg = GetThisChatTelegram(Update.Message.Chat.Id);

        if (ChatTg is null)
            CreateChatIfNull(idChat: Update.Message.Chat.Id,
                title: Update.Message.Chat.Title ?? Update.Message.Chat.FirstName ?? string.Empty);

        foreach (var user in Update.Message.NewChatMembers)
        {
            CreateAccount(user);
            CreateEventJoin(user);
        }
        return Task.FromResult(true);
    }

    private void CreateEventJoin(User user)
    {
        var newEvent = new EventJoined
        {
            IdTelegram = user.Id,
            ChatId = Update.Message?.Chat.Id,
            DateTime = DateTime.Now
        };
        Repository.Joins.Add(newEvent);
    }
    
    protected void CreateAccount(User user)
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
}