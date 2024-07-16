using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class SetMeManager: IExecutable<bool>
{
    public ITelegramBotClient BotClient = default!;
    public Update Update = default!;
    public SandBoxRepository Repository = default!;
    public string Secret = default!;
    
    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);
        
        SendMessage(BuildMessage());
        return Task.FromResult(true);
    }
    private Task SendMessage(string message)
    {
        BotClient.SendTextMessageAsync(chatId:Update.Message!.Chat.Id,
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