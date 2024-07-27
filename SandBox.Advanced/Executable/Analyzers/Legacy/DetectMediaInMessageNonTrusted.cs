using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using SandBox.Advanced.Utils.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SandBox.Advanced.Executable.Analyzers.DeleteableMessages;

public class DetectMediaInMessageNonTrusted(SandBoxRepository repository, 
    ITelegramBotClient botClient, long idChat) : IAnalyzer
{
    public void Execute(Message message)
    {
        if (message.From is null || message.Chat.Id != idChat)
            return;

        var account = repository.Accounts.GetByIdAsync(message.From.Id).Result;

        if (account is null)
            return;
        
        if (account.IsTrustedProfile() || botClient.IsUserAdminInChat(userId: message.From.Id,
                chatId: message.Chat.Id))
            return;

        if (message.Type is not (MessageType.Animation or MessageType.Audio or MessageType.Document
            or MessageType.Location
            or MessageType.Photo or MessageType.Poll or MessageType.Sticker or MessageType.Video
            or MessageType.Voice)) return;
        
        NotifyManagers(message);
        botClient.DeleteMessageAsync(chatId: message.Chat.Id, messageId: message.MessageId);
        
        // т.к. этот скрипт сработал как спам, даем указанием следующим стриптам не проверять
        message.Text = null;
    }
    
    private Task NotifyManagers(Message originalMessage)
    {
        foreach (var id in repository.Accounts.GetManagers().Result)
        {
            try
            {
                var message = BuildNotifyMessage(originalMessage);

                botClient.ForwardMessageAsync(chatId: id.IdTelegram,
                    fromChatId: originalMessage.Chat.Id,
                    messageId: originalMessage.MessageId,
                    disableNotification: true);
                
                botClient.SendTextMessageAsync(chatId: id.IdTelegram,
                    text: message,
                    disableNotification: true);
            }
            catch
            {
                // ignored
            }
        }
        
        return Task.CompletedTask;
    }

    private string BuildNotifyMessage(Message message)
    {
        return
            $"\ud83d\udc7e Удалено сообщение от пользователя {message.From?.Id} (@{message.From?.Username}) в чате # {message.Chat.Id} - ({message.Chat.Title ?? message.Chat.FirstName}) со " +
            $"следующем типом содержания: \n\n{message.Type} \n\n" +
            $"ℹ️ Это сообщение удалено, потому что в сообщении находятся медиа-материалы (голос, видео, картинка и т.д.) и пользователь находится в беседе не так долго.";
    }
    
}