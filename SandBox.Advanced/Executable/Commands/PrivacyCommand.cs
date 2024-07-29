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
            $"\n\n \ud83d\udc64 Мы запоминаем, кто вы и как часто пишете, чтобы знать кто бот, а кто действительно человек" +
            $"\n\n \ud83d\udcac Ваши сообщения храним для обучения нашего ИИ и улучшения антиспама." +
            $"\n\n \u2705 Нам не интересен ваш контент, мы его не читаем." +
            $"\n\n ℹ\ufe0f Если не согласны с этим, просто не пользуйтесь ботом и не добавляйте его в чаты.";
    }
}