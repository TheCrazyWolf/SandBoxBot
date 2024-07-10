using SandBoxBot.Commands.Base;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.About;

public class StartCommand : BlackBase, ICommand
{
    public async Task Execute(Message message, CancellationToken cancellationToken)
    {
        string version = "1.1.0";

        await BotClient.SendTextMessageAsync(message.Chat.Id, $"\ud83c\udfaf Анти-спам фильтр\n\nВерсия: {version}" +
                                                              $"\n\nРазработано @kulagin_alex \n\nФильтр может излишне реагировать на обычные сообщения" +
                                                              $"\nЕсли Ваши сообщения удаляются из беседы, мы об этом уже вкурсе. \n\nЧерез небольшое время органичение будет снято. Простите \ud83d\ude22",
            cancellationToken: cancellationToken);
    }

    public StartCommand(ITelegramBotClient botClient, SandBoxRepository repository) : base(botClient, repository)
    {
    }
}