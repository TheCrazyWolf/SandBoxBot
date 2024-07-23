using SandBox.Advanced.Database;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Executable.Services;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectNonWorkingTime(SandBoxRepository repository, ITelegramBotClient botClient, long chatId) : TimeServer, IAnalyzer
{
    public bool Execute(Message message)
    {
        if (message.From is null || message.Chat.Id != chatId)
            return false;

        var account = repository.Accounts.GetById(message.From.Id).Result;
        
        if (account != null && (account.IsManagerThisBot || botClient.IsUserAdminInChat(userId: message.From.Id,
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