using SandBoxBot.Commands.Base.Interfaces;
using SandBoxBot.Commands.Base.Messages;
using SandBoxBot.Database;
using SandBoxBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Events.OnLogon;

public class InvitedUserActivity(ITelegramBotClient botClient, SandBoxRepository repository, Message? message)
    : EventMessageCommand(botClient, repository, message), ICommand
{
    
    public async Task Execute(CancellationToken cancellationToken)
    {
        await RegisterIfNewUser();
    }

    private async Task RegisterIfNewUser()
    {
        if (Message?.NewChatMembers is null)
            return;
        
        foreach (var user in Message.NewChatMembers)
        {
            if (await Repository.Accounts.ExistAccount(user.Id))
                continue;

            var newAccount = new Account
            {
                IdAccountTelegram = user.Id,
                ChatId = Message.Chat.Id,
                LastActivity = DateTime.Now,
                DateTimeJoined = DateTime.Now,
                UserName = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsAdmin = false
            };

            await Repository.Accounts.Add(newAccount);
        }
    }

   
}