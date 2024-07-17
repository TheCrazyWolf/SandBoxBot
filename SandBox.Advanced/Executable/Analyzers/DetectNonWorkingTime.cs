using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using Telegram.Bot;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectNonWorkingTime : SandBoxHelpers, IExecutable<bool>
{
    private bool _isAdminOrManager;
    private bool _isWorkTime;
    private static DateTime _lastNotifyied;

    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);

        AccountDb = Repository.Accounts.GetById(Update.Message.From.Id).Result;
        _isAdminOrManager = IfThisUserIsManager(Update.Message.From.Id, Update.Message.Chat.Id).Result;
        _isWorkTime = IsWorkTime();

        if (IfCanBeSendedMessage()) return Task.FromResult(true);

        SendMessage(Update.Message.Chat.Id);
        
        DeleteThisMessage(chatId: Update.Message.Chat.Id, messageId: Update.Message.MessageId);

        return Task.FromResult(true);
    }

    private void SendMessage(long idChat)
    {
        if ((DateTime.Now - _lastNotifyied).TotalHours <= 8)
            return;

        _lastNotifyied = DateTime.Now;
        BotClient.SendTextMessageAsync(chatId: idChat, text: BuildMessage());
    }
    
    private bool IfCanBeSendedMessage()
    {
        if (_isWorkTime)
            return _isWorkTime;

        if (_isAdminOrManager)
            return _isAdminOrManager;

        return false;
    }

    private string BuildMessage()
    {
        return
            "\u2764\ufe0f Мы хотим помогать Вам круглосуточно\n" +
            "\n\u2705 Но получить ответы на вопросы Вы можете в рабочее время: \n\n\u23f0 ПН-ЧТ С 8.00 по 16.30, ПТ до 15.30";
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