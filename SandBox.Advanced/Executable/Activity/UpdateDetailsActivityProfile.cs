using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Activity;

public sealed class UpdateDetailsActivityProfile(SandBoxRepository repository) : IAnalyzer
{
    public void Execute(Message message)
    {
        if (message.From?.Id is null)
            return;

        var chatTg = repository.Chats.GetByIdAsync(message.Chat.Id).Result;

        if (chatTg is null)
            repository.Chats.AddAsync(message.Chat.CreateChatDb());
        
        var account = repository.Accounts.GetById(message.From.Id).Result;

        if (account is not null)
        {
            repository.Accounts.UpdateDetails(account, message.From);
            return;
        }

        repository.Accounts.Add(message.From.CreateAccountDb());
        
    }
    
}