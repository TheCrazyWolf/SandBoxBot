using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Activity;

public sealed class UpdateDetailsActivityProfile(SandBoxRepository repository, ITelegramBotClient botClient) : IAnalyzer
{
    public bool Execute(Message message)
    {
        if (message.From?.Id is null)
            return false;

        var chatTg = repository.Chats.GetById(message.Chat.Id).Result;

        if (chatTg is null)
            repository.Chats.Add(message.Chat.CreateChatDb());
        
        var account = repository.Accounts.GetById(message.From.Id).Result;

        if (account is not null)
        {
            repository.Accounts.UpdateDetails(account, message.From);
            return false;
        }

        repository.Accounts.Add(message.From.CreateAccountDb());

        return true;
    }
    
}