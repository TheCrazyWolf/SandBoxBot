using Telegram.Bot;

namespace SandBox.Advanced.Utils;

public static class TelegramUtils
{
    public static bool IsUserAdminInChat(this ITelegramBotClient botClient, long userId, long chatId)
    {
        try
        {
            var chatMember = botClient.GetChatAdministratorsAsync(chatId: chatId).Result;
            return chatMember.Any(x => x.User.Id == userId);
        }
        catch
        {
            return false;
        }
    }
}