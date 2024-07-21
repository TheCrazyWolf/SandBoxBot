using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectMediaInMessageNonTrusted(SandBoxRepository repository, ITelegramBotClient botClient) : IAnalyzer
{
    public bool Execute(Message message)
    {
        if (message.From is null)
            return false;

        var account = repository.Accounts.GetById(message.From.Id).Result;

        if (account is null)
            return false;
        
        if (account.IsTrustedProfile())
            return false;

        if (message.Type is not (MessageType.Animation or MessageType.Audio or MessageType.Document
            or MessageType.Location
            or MessageType.Photo or MessageType.Poll or MessageType.Sticker or MessageType.Video
            or MessageType.Voice)) return true;
        
        
        botClient.DeleteMessageAsync(chatId: message.Chat.Id, messageId: message.MessageId);
        NotifyManagers(message);

        return true;
    }
    
    private void NotifyManagers(Message originalMessage)
    {
        foreach (var id in repository.Accounts.GetManagers().Result)
        {
            try
            {
                var message = BuildNotifyMessage(originalMessage);

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
            $"\ud83d\udc7e Удалено сообщение от пользователя {message.From?.Id} (@{message.From?.Username}) со " +
            $"следующем типом содержания: \n\n{message.Type} \n\n\u26a0\ufe0fМы недоверяем пользователям, которые состоят в беседе недавно и еще не прошли проверку на бота, сообщениям, которые содержат медиа файлы";
    }
    
}