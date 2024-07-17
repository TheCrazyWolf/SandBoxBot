using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectNonWorkingTime : SandBoxHelpers, IExecutable<bool>
{
    private bool _isOverride = false;
    private bool _isWorkTime = false;
    private static Message? _lastMessage = default!;
    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);

        AccountDb = Repository.Accounts.GetById(Update.Message.From.Id).Result;
        _isOverride = CanBeOverrideRestriction(Update.Message.From.Id, Update.Message.Chat.Id).Result;
        _isWorkTime = IsWorkTime();

        if (!CanBeSendedMessage())
        {
            SendMessage();
            DeleteThisMessage();
        }

        return Task.FromResult(true);
    }

    private Task SendMessage()
    {
        if (_lastMessage is null)
        {
            _lastMessage = BotClient.SendTextMessageAsync(chatId: Update.Message!.Chat.Id,
                text: BuildMessage()).Result;
        }
        else if ((DateTime.Now.AddHours(-4) - _lastMessage.Date).TotalHours <= 8)
        {
            return Task.CompletedTask;
        }
        else
        {
            BotClient.DeleteMessageAsync(chatId:_lastMessage.MessageId, 
                messageId: _lastMessage.MessageId );
            _lastMessage = BotClient.SendTextMessageAsync(chatId: Update.Message!.Chat.Id,
                text: BuildMessage()).Result;
        }
        
        return Task.CompletedTask;
    }
    
    private Task DeleteThisMessage()
    {
        BotClient.DeleteMessageAsync(chatId: Update.Message!.Chat.Id,
            messageId: Update.Message.MessageId
        );
        return Task.CompletedTask;
    }
    

    private bool CanBeSendedMessage()
    {
        if (_isWorkTime)
            return _isWorkTime;

        if (_isOverride)
            return _isOverride;

        return false;
    }

    private string BuildMessage()
    {
        return
            "Мы хотим помогать Вам круглосуточно \u2764\ufe0f\n" +
            "Но получить ответы на вопросы Вы можете в рабочее время: ПН-ЧТ С 8.00 по 16.30, ПТ до 15.30 \u2705";
    }

    private bool IsWorkTime()
    {
        
        if (DateTime.Now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return false;
        
        var timeOnlyNow = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay);

        var start = new TimeOnly(08, 00, 00);
        var end = new TimeOnly(16, 30, 00);
        var endShort = new TimeOnly(15, 30, 00);

        if (!(timeOnlyNow >= start && timeOnlyNow <= end))
            return false;

        if (DateTime.Now.DayOfWeek is DayOfWeek.Friday && !(timeOnlyNow >= start && timeOnlyNow <= endShort))
            return false;

        return true;
    }
}