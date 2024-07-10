using SandBoxBot.Commands.Base;
using SandBoxBot.Database;
using SandBoxBot.Models;
using SandBoxBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace SandBoxBot.Commands.Black;

public class WelcomeMessageCommand(ITelegramBotClient botClient, SandBoxRepository repository) : BlackBase(
    botClient,
    repository), ICommand
{
    public async Task Execute(Message message, CancellationToken cancellationToken)
    {
        if (message.NewChatMembers is not null)
            await RegisterIfNewUser(message.NewChatMembers, message.Chat.Id);
        
        await SendWelcomeMessageIfNewUser(message.Chat.Id, message.From?.FirstName);
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
        if(!System.IO.File.Exists("Welcome.txt"))
            return;

        var content = await System.IO.File.ReadAllTextAsync("Welcome.txt");

        if (string.IsNullOrEmpty(content)) 
            return;
        
        try
        {
            await BotClient.SendTextMessageAsync(idChat,
                $"\ud83e\udd1f {GetGreeting(DateTime.Now)}, {firstname}\n\n{content}"
            );
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