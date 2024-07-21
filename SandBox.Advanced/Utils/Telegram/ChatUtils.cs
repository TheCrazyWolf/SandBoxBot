using SandBox.Models.Telegram;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Utils.Telegram;

public static class ChatUtils
{
    public static ChatTg CreateChatDb(this Chat chat)
    {
        return new ChatTg()
        {
            IdChat = chat.Id,
            Title = chat.Title ?? chat.FirstName ?? string.Empty
        };
    }
}