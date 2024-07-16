using SandBox.Advanced.Database;
using SandBox.Models.Events;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Activity;

public class UpdateDetailsActivityOnJoined(
    ITelegramBotClient botClient,
    Update update,
    SandBoxRepository repository)
    : UpdateDetailsActivityProfile(botClient, update, repository)
{
    public override Task<bool> Execute()
    {
        if (update.Message?.NewChatMembers is null)
            return Task.FromResult(false);

        _chatTg = GetThisChatTelegram();

        if (_chatTg is null)
            CreateChatIfNull();

        foreach (var user in update.Message.NewChatMembers)
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
            ChatId = update.Message?.Chat.Id,
            DateTime = DateTime.Now
        };

        return repository.Joins.Add(newEvent).Result;
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

        repository.Accounts.Add(newAccount);
        return Task.CompletedTask;
    }
}