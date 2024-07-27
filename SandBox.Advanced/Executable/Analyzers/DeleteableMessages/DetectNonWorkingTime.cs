using SandBox.Advanced.Database;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Executable.Services;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectNonWorkingTime(SandBoxRepository repository, 
    ITelegramBotClient botClient, long chatId) : TimeServer, IAnalyzer
{
    public void Execute(Message message)
    {
        if (message.From is null || message.Chat.Id != chatId)
            return;

        var account = repository.Accounts.GetByIdAsync(message.From.Id).Result;
        
        if (account != null && (account.IsManagerThisBot || botClient.IsUserAdminInChat(userId: message.From.Id,
                chatId: message.Chat.Id)))
        {
            return;
        }
        else if (!WorkTimeChatTimer.IsWorkTime())
        {
            botClient.DeleteMessageAsync(chatId: message.Chat.Id, messageId: message.MessageId);
            // т.к. этот скрипт сработал как спам, даем указанием следующим стриптам не проверять
            message.Text = null;
            return;
        }
        
        
    }
    
}