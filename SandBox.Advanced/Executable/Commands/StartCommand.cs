using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Services.Text;
using SandBox.Models.Blackbox;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class StartCommand(
    ITelegramBotClient botClient,
    Update update,
    SandBoxRepository repository) : IExecutable
{

    public Task Execute()
    {
        if (update.Message?.From is null)
            return Task.CompletedTask;
        
        SendMessage(BuildMessage());
        return Task.CompletedTask;
    }
    private Task SendMessage(string message)
    {
        botClient.SendTextMessageAsync(chatId:update.Message!.Chat.Id,
            text: message,
            disableNotification: true);
        return Task.CompletedTask;
    }

    private string BuildMessage()
    {
        string version = "1.5.0";

        return
            $"\ud83c\udfaf Анти-подработка (Антиспам)\n\nВерсия: {version}" +
            $"\n\nРазработано @kulagin_alex, by samgk.ru \n\nАнтиспам реагирует на контекст сообщений на основе машинного обучения, " +
            $"поведению пользователей в чате. Возможны ошибочные распознавания." +
            $"\n\nЕсли Вы столкнулись с ограничениями в беседе, мы об этом уже в курсе. В скором времени ограничения будут сняты";
    }
    
}