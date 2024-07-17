using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Services.Text;
using SandBox.Models.Blackbox;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class StartCommand : SandBoxHelpers, IExecutable<bool>
{
    private bool _canByPassRestrict;
    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);

        _canByPassRestrict =
            CanBeOverrideRestriction(idTelegram: Update.Message.From.Id, idChat: Update.Message.Chat.Id).Result;

        AccountDb = Repository.Accounts.GetById(Update.Message.From.Id).Result;

        SendMessage(idChat: Update.Message.Chat.Id, message: BuildMessage());
        return Task.FromResult(true);
    }

    private void SendMessage(long idChat, string message)
    {
        BotClient.SendTextMessageAsync(chatId: idChat,
            text: message,
            disableNotification: true);
    }

    private string BuildMessage()
    {
        string version = "1.5.0";
        string msg = IsRestrictedUser()
            ? "\n\n\ud83e\udd2f Очень жаль, что Вам пришлось столкнутся с ограничениями нашим суровым анти-спамом. Предлагаю это исправить - наберите команду /captcha мне личные сообщения"
            : "\n\n\u2705 На текущий момент на Вашем аккаунте нет ограничений на антиспам фильтре";
        return
            $"\ud83c\udfaf Анти-подработка (Антиспам)\n\nВерсия: {version}" +
            $"\n\nРазработано @kulagin_alex, by samgk.ru \n\nАнтиспам реагирует на контекст сообщений на основе машинного обучения, " +
            $"поведению пользователей в чате. Возможны ошибочные распознавания." +
            $"{msg}";
    }

    private bool IsRestrictedUser()
    {
        if (AccountDb is null)
            return true;

        if (AccountDb.IsSpamer)
            return AccountDb.IsSpamer;

        if (_canByPassRestrict)
            return !_canByPassRestrict;

        return false;
    }
}