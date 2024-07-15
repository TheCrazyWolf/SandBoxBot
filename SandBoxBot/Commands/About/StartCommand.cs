using SandBoxBot.Commands.Base.Interfaces;
using SandBoxBot.Commands.Base.Messages;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.About;

public class StartCommand(ITelegramBotClient botClient, SandBoxRepository repository, Message? message) : 
    EventMessageCommand(botClient, repository, message), ICommand
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        if(Message is null)
            return;
        
        string version = "1.2.0";

        await BotClient.SendTextMessageAsync(Message.Chat.Id, 
            $"\ud83c\udfaf Анти-спам фильтр\n\nВерсия: {version}" +
            $"\n\nРазработано @kulagin_alex, by samgk.ru \n\nАнти-спам фильтр работает по ключевым словам и поведению пользователя в чате" +
            $"\n\nБот может сурово реагировать на Ваши некоторые сообщения в беседе" +
            $"\n\nЕсли Вы заметили, что Ваши сообщения удаляются из беседы, мы об этом уже вкурсе. Через небольшое время органичение будет снято. Простите \ud83d\ude22",
            cancellationToken: cancellationToken);
    }
}