using Newtonsoft.Json.Linq;
using SandBox.Advanced.Database;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectNonWorkingTime(SandBoxRepository repository, ITelegramBotClient botClient) : TimeServer, IAnalyzer
{
    
    public bool Execute(Message message)
    {
        if (message.From is null)
            return false;

        var account = repository.Accounts.GetById(message.From.Id).Result;
        
        if (account != null && (account.IsTrustedProfile() || botClient.IsUserAdminInChat(userId: message.From.Id,
                chatId: message.Chat.Id)))
        {
            return false;
        }
        else if (!WorkTimeChatTimer.IsWorkTime())
        {
            botClient.DeleteMessageAsync(chatId: message.Chat.Id, messageId: message.MessageId);
            return true;
        }

        return false;
    }
    
    
}