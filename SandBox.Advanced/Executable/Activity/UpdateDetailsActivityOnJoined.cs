using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Activity;

public class UpdateDetailsActivityOnJoined(SandBoxRepository repository, ITelegramBotClient botClient):  IAnalyzer
{
    public bool Execute(Message message)
    {
        if (message.NewChatMembers is null)
            return false;
        
        var chatTg = repository.Chats.GetById(message.Chat.Id).Result;

        if (chatTg is null)
            repository.Chats.Add(message.Chat.CreateChatDb());
        
        foreach (var user in message.NewChatMembers)
        {
            repository.Accounts.Add(user.CreateAccountDb());
            repository.Joins.Add(user.CreateEventJoinFromUser(message.Chat.Id));
        }

        return true;
    }
    
    // Сделать проверку на спам атаку заходов
    
}