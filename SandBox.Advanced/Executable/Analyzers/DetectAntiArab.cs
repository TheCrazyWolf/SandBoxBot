using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectAntiArab(SandBoxRepository repository, ITelegramBotClient botClient) : IAnalyzer 
{
    public bool Execute(Message message)
    {
        if(message.NewChatMembers is not null)
            return false;

        // IMPLEMEMT
        return true;
    }
}
