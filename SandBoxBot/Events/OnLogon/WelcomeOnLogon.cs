using SandBoxBot.Commands.Base.Interfaces;
using SandBoxBot.Commands.Base.Messages;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Events.OnLogon;

public class WelcomeOnLogon(ITelegramBotClient botClient, SandBoxRepository repository, Message? message)
    : EventMessageCommand(botClient, repository, message), ICommand
{
    private static DateTime _dateTimeLastSended;
    private static Message? _lastMessageSendedWelcome;
    
    public async Task Execute(CancellationToken cancellationToken)
    {
        if (Message?.NewChatMembers is null)
            return;

        await new InvitedUserActivity(BotClient, Repository, Message).Execute(cancellationToken);

        if ((DateTime.Now - _dateTimeLastSended).TotalMinutes >= 59)
        {
           await DeleteLastMessageIfNewUser();
           await SendWelcomeMessageIfNewUser(Message.Chat.Id, Message.From?.FirstName);
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
            
            _lastMessageSendedWelcome = await BotClient.SendTextMessageAsync(idChat,
                $"\ud83e\udd1f {GetGreeting(DateTime.Now)}, {firstname}\n\n{content}",
                disableNotification: true
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

    private async Task DeleteLastMessageIfNewUser()
    {
        if(_lastMessageSendedWelcome is null)
            return;

        try
        {
            await BotClient.DeleteMessageAsync(chatId:
                _lastMessageSendedWelcome.Chat.Id,
                _lastMessageSendedWelcome.MessageId);

            _lastMessageSendedWelcome = null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}