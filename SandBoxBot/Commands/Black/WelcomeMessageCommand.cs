using SandBoxBot.Commands.Base;
using SandBoxBot.Commands.Base.Interfaces;
using SandBoxBot.Commands.Base.Messages;
using SandBoxBot.Database;
using SandBoxBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
namespace SandBoxBot.Commands.Black;

public class WelcomeMessageCommand(ITelegramBotClient botClient, SandBoxRepository repository, Message? message)
    : EventMessageCommand(botClient, repository, message), ICommand
{
    private static DateTime _dateTimeLastSended = new DateTime();

    public async Task Execute(CancellationToken cancellationToken)
    {
        if (Message?.NewChatMembers is null)
            return;

        await RegisterIfNewUser(Message.NewChatMembers, Message.Chat.Id);

        if ((DateTime.Now - _dateTimeLastSended).TotalMinutes >= 59)
            await SendWelcomeMessageIfNewUser(Message.Chat.Id, Message.From?.FirstName);
    }

    private async Task RegisterIfNewUser(User[] invitedUsers, long chatId)
    {
        foreach (var user in invitedUsers)
        {
            if (await Repository.Accounts.ExistAccount(user.Id))
                continue;

            var newAccount = new Account()
            {
                IdAccountTelegram = user.Id,
                ChatId = chatId,
                LastActivity = DateTime.Now,
                DateTimeJoined = DateTime.Now,
                UserName = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            await Repository.Accounts.Add(newAccount);
        }
    }

    private async Task SendWelcomeMessageIfNewUser(long idChat, string? firstname)
    {
        if (!System.IO.File.Exists("Welcome.txt"))
            return;

        var content = await System.IO.File.ReadAllTextAsync("Welcome.txt");

        if (string.IsNullOrEmpty(content))
            return;

        try
        {
            await BotClient.SendTextMessageAsync(idChat,
                $"\ud83e\udd1f {GetGreeting(DateTime.Now)}, {firstname}\n\n{content}"
            );

            _dateTimeLastSended = DateTime.Now;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private string GetGreeting(DateTime time)
    {
        var hour = time.Hour;

        return hour switch
        {
            >= 6 and < 12 => "Доброе утро",
            >= 12 and < 18 => "Добрый день",
            >= 18 and < 24 => "Добрый вечер",
            _ => "Доброй ночи"
        };
    }
}