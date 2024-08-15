using SandBox.Models.Chats;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Utils.Telegram;

public static class ChatUtils
{
    public static ChatProps CreateChatDb(this Chat chat)
    {
        return new ChatProps
        {
            IdChat = chat.Id,
            Title = chat.Title,
            Type = chat.Type,
            FirstName = chat.FirstName,
            LastName = chat.LastName,
            Username = chat.Username,
        };
    }
}