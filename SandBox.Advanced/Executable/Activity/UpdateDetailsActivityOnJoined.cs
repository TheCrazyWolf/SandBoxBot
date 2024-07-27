using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Activity;

public class UpdateDetailsActivityOnJoined(SandBoxRepository repository):  IAnalyzer
{
    public void Execute(Message message)
    {
        if (message.NewChatMembers is null)
            return;
        
        var chatTg = repository.Chats.GetByIdAsync(message.Chat.Id).Result;

        if (chatTg is null)
            repository.Chats.AddAsync(message.Chat.CreateChatDb());
        
        foreach (var user in message.NewChatMembers)
        {
            repository.Accounts.Add(user.CreateAccountDb());
            repository.Joins.Add(user.CreateEventJoinFromUser(message));
        }
        
    }
    
    // Сделать проверку на спам атаку заходов
    
}