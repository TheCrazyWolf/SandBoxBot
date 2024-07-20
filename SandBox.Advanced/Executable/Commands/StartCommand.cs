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

        var account = repository.Accounts.GetById(message.From.Id).Result;

        if (account is null)
            return;

        SendMessage(idChat: message.Chat.Id, message: BuildMessage(!account.IsTrustedProfile()));
    }
    
    private void SendMessage(long idChat, string message)
    {
        botClient.SendTextMessageAsync(chatId: idChat,
            text: message,
            disableNotification: true);
    }

    private string BuildMessage(bool isTrusted)
    {
        string version = "1.5.0";
        string msg = isTrusted
            ? "\n\n\ud83e\udd2f Очень жаль, что Вам пришлось столкнутся с ограничениями нашим суровым анти-спамом. Предлагаю это исправить - наберите команду /captcha мне личные сообщения"
            : "\n\n\u2705 На текущий момент на Вашем аккаунте нет ограничений на антиспам фильтре";
        return
            $"\ud83c\udfaf Анти-подработка (Антиспам)\n\nВерсия: {version}" +
            $"\n\nРазработано @kulagin_alex, by samgk.ru \n\nАнтиспам реагирует на контекст сообщений на основе машинного обучения, " +
            $"поведению пользователей в чате. Возможны ошибочные распознавания." +
            $"{msg}";
    }
}