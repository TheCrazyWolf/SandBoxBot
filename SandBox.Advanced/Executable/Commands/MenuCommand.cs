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

        const string usage = """
                                 <b><u>Меню бота</u></b>:
                                 /start                   - О боте, версия и проверка ограничение
                                 /check Текст сообщения   - Проверка текста на фильтры
                                 /captcha                 - Пройти проверку на робота
                                 /add Слово1 Слово2       - Заблокировать слова
                                 /del Слово1 Слово2       - Удалить слова
                                 /question Текст вопроса  - Поиск вопроса в базе знаний
                                 /time                    - Проверка серверного времени
                             """;
        
        botClient.SendTextMessageAsync(message.Chat, usage, parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }
}