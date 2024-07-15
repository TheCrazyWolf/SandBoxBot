using SandBoxBot.Commands.Base.Interfaces;
using SandBoxBot.Commands.Base.Messages;
using SandBoxBot.Database;
using SandBoxBot.Models;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace SandBoxBot.Events.OnLogon;

public class UpdateActivityUser(ITelegramBotClient botClient, SandBoxRepository repository, Message? message)
    : EventMessageCommand(botClient, repository, message), ICommand
{
    
    public async Task Execute(CancellationToken cancellationToken)
    {
        await UpdateLastActivtityUser();
    }

    private async Task UpdateLastActivtityUser()
    {
        if (Message?.From is null)
            return;

        var accountDetails = await Repository.Accounts.Get(Message.From.Id);
        
        if(accountDetails is null)
            return;
        
        accountDetails.LastActivity = DateTime.Now;
        accountDetails.FirstName = Message?.From.FirstName ?? string.Empty;
        accountDetails.LastName = Message?.From.LastName ?? string.Empty;
        accountDetails.UserName = Message?.From.Username ?? string.Empty;

        await Repository.Accounts.Update(accountDetails);

    }
    
}