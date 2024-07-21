using SandBox.Models.Events;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Utils;

public static class EventUtils
{
    public static EventContent GenerateEventFromContent(this Message message, bool isSpam = default)
    {
        return new EventContent
        {
            IsSpam = isSpam,
            ChatId = message.Chat.Id,
            DateTime = DateTime.Now,
            Content = message.Text ?? string.Empty,
            IdTelegram = message.From?.Id
        };
    }
}