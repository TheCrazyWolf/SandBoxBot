using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class PrivacyCommand(ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/privacy";

    public override void Execute(Message message)
    {
        if (message.From is null)
            return;
        
        TrySendMessage(idChat: message.Chat.Id, message: BuildMessage());
    }
    
    private async void TrySendMessage(long idChat, string message)
    {
        try
        {
            await botClient.SendTextMessageAsync(chatId: idChat,
                text: message,
                disableNotification: true);
        }
        catch
        {
            // ingored
        }
    }

    private string BuildMessage()
    {
        return
            $"\ud83c\udfaf Политика конфиденциальности. Человеческим языком :)" +
            $"\n\n👤\ud83d\udc64 Храним информацию о пользователях, чтобы запомнить Вас как часто пишите сообщения, отправлять Вам данные." +
            $"\n\n \ud83d\udcac Храним сообщения, чтобы можно было использовать для машинного обучения, чтобы распознавать спам в будущем точнее" +
            $"\n\n ✅ Нам не интересны Ваши сообщения в других целях, мы их не читаем" +
            $"\n\n ℹ️ Пожалуйста предупреждайте пользователей, если Вы не согласны с такой политикой, не используйте бота и не добавляйте его в беседу";
    }
}