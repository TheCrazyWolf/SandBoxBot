using SandBox.Advanced.Configs;
using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectTelegramBotInChat(SandBoxRepository repository, ITelegramBotClient botClient, long idChat) : IAnalyzer
{
    public bool Execute(Message message)
    {
        if (message.Chat.Id != idChat)
            return false;

        if (message.NewChatMembers is not null)
        {
            foreach (var user in message.NewChatMembers)
            {
                if(!user.IsBot || user.Id == BotConfiguration.IdBot)
                    continue;
                
                NotifyManagers(message, BuildNotifyMessage(user));
                botClient.DeleteMessageAsync(chatId: message.Chat.Id, messageId: message.MessageId);
                botClient.BanChatMemberAsync(chatId: message.Chat.Id, userId: user.Id);
            }

            return true;
        }

        if (message.From is null)
            return false;

        if (!message.From.IsBot || message.From.Id == BotConfiguration.IdBot) return true;
        
        NotifyManagers(message, BuildNotifyMessage(message));
        botClient.DeleteMessageAsync(chatId: message.Chat.Id, messageId: message.MessageId);
        botClient.BanChatMemberAsync(chatId: message.Chat.Id, userId: message.From.Id);

        return true;
    }
    
    private void NotifyManagers(Message originalMessage, string message)
    {
        foreach (var id in repository.Accounts.GetManagers().Result)
        {
            try
            {
                botClient.SendTextMessageAsync(chatId: id.IdTelegram,
                    text: message,
                    disableNotification: true);
            }
            catch
            {
                // ignored
            }
        }
    }

    private string BuildNotifyMessage(Message message)
    {
        return
            $"\ud83d\udc7e Пользователь # {message.From?.Id} (@{message.From?.Username}) (бот)" +
            $"проявил активность, но оказался ботом\n\n\u2705 Удален из чата";
    }
    
    private string BuildNotifyMessage(User user)
    {
        return
            $"\ud83d\udc7e Пользователь # {user.Id} (@{user.Username}) (бот) " +
            $"был добавлен в чат, в котором запрещено добавлять телеграм ботов. (Могут устраивать рейды)\n\n\u2705 Удален из чата";
    }
    
}