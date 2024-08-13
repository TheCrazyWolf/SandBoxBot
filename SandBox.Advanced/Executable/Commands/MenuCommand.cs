using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Commands;

public class MenuCommand(ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/menu";

    public override void Execute(Message message)
    {
        if (message.From is null)
            return;

        TrySendMessage(message.Chat.Id, BuildUsageMessage());
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

    private string BuildUsageMessage()
    {
        return
            $"[Меню бота]\n" +
            $"Доступные Вам команды\n" +
            $"/check (Текст) (или на ответом на сообщение) - проверить текст на срабатывание ML\n" +
            $"/acheck - получить информацию об аккаунте в беседе, (можно ответом на сообщение)\n" +
            $"/captcha - пройти проверку на робота\n" +
            $"/privacy - политика о конфиденциальности\n\n" +
            $"Админские\n" +
            $"/purge (ответом на сообщение) - очистка чата до момента\n" +
            $"/time - проверка серверного времени\n";
    }
}