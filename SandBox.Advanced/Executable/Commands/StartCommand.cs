using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class StartCommand(SandBoxRepository repository, ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/start";

    public override void Execute(Message message)
    {
        if (message.From is null)
            return;

        var account = repository.Accounts.GetByIdAsync(message.From.Id).Result;

        if (account is null)
            return;

        TrySendMessage(idChat: message.Chat.Id, message: BuildMessage());
    }
    
    private void TrySendMessage(long idChat, string message)
    {
        try
        {
            botClient.SendTextMessageAsync(chatId: idChat,
                text: message,
                disableNotification: true);
        }
        catch 
        {
            // ignored
        }
    }

    private string BuildMessage()
    {
        string version = "3.1";
        return
            $"\ud83c\udfaf Анти-спам\n\nВерсия: {version}" +
            $"\n\nРазработано @kulagin_alex, by samgk.ru \n\n" +
            $"Добавь меня в беседу, дай права администратора для удаления сообщений и блокировки пользователей.\n\n" +
            $"🗓 О том, какие данные я собираю - /privacy\n\n" +
            $"🤖 Могу ошибаться, но в основном обучен на больших данных (русскоязычных). Для отправки логов сюда, " +
            $"сначала напиши в беседу (которую пригласил) любое сообщение, чтобы я знал, что ты админ.";
    }
}