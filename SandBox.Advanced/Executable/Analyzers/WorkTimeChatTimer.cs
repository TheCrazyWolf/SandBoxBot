using Newtonsoft.Json.Linq;
using Telegram.Bot;

namespace SandBox.Advanced.Executable.Analyzers;

public static class WorkTimeChatTimer
{
    private static bool IsRun { get; set; }
    public static ITelegramBotClient BotClient { get; set; } = default!;

    private static readonly TimeOnly StartWorkTime = new(08, 00, 00);
    private static readonly TimeOnly EndWorkTimeMonday = new(19, 00, 00);
    private static readonly TimeOnly EndWorkTimeDefault = new(16, 00, 00);
    
    // CHAT приемки -1001941895047
    // -4286170959
    private static readonly long ChatId = -4286170959;

    private static DateTime _lastSendStartMessageDay;
    private static DateTime _lastSendEndMessageDay;

    public static void Rune()
    {
        if (IsRun)
            return;

        IsRun = true;

        Task.Run(CheckIn);
    }

    private static async Task CheckIn()
    {
        while (true)
        {
            var now = DateTime.Now;
            //var now = new DateTime(2024, 07, 18, 08, 00, 54);
            if (ShouldSendStartMessage(now) == true)
            {
                await BotClient.SendTextMessageAsync(chatId: ChatId,
                    text: BuildMessageIfTimeWorkStarted());
                _lastSendStartMessageDay = now;
            }

            if (ShouldSendEndMessage(now))
            {
                await BotClient.SendTextMessageAsync(chatId: ChatId,
                    text: BuildMessageIfTimeWorkEnd());
                _lastSendEndMessageDay = now;
            }

            await Task.Delay(1000); // Проверка каждую секунду
        }
    }

    private static bool ShouldSendStartMessage(DateTime now)
    {
        var timeOnlyNow = TimeOnly.FromTimeSpan(now.TimeOfDay);

        if (_lastSendStartMessageDay.Date == now.Date)
            return false;

        if (now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return false;

        return timeOnlyNow.Hour == StartWorkTime.Hour && timeOnlyNow.Minute == StartWorkTime.Minute;
    }

    private static bool ShouldSendEndMessage(DateTime now)
    {
        var timeOnlyNow = TimeOnly.FromTimeSpan(now.TimeOfDay);

        if (_lastSendEndMessageDay.Date == now.Date)
            return false;

        if (now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return false;

        if (now.DayOfWeek is DayOfWeek.Monday)
            return timeOnlyNow.Hour == EndWorkTimeMonday.Hour && timeOnlyNow.Minute == EndWorkTimeMonday.Minute;

        return timeOnlyNow.Hour == EndWorkTimeDefault.Hour && timeOnlyNow.Minute == EndWorkTimeDefault.Minute;
    }


    public static bool IsWorkTime()
    {
        if (DateTime.Now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return false;

        var timeOnlyNow = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay);

        if (DateTime.Now.DayOfWeek is DayOfWeek.Monday &&
            (timeOnlyNow >= StartWorkTime && timeOnlyNow <= EndWorkTimeMonday))
            return true;

        if (!(timeOnlyNow >= StartWorkTime && timeOnlyNow <= EndWorkTimeDefault))
            return false;

        return true;
    }

    private static string BuildMessageIfTimeWorkEnd()
    {
        return
            "\u2764\ufe0f Мы хотим помогать Вам круглосуточно\n" +
            "\n\u2705 Но получить ответы на вопросы Вы можете в рабочее время: \n\n\u23f0 ПН с 8.00 по 19.00, ВТ-ПТ до 16.00 (Самарское)";
    }

    private static string BuildMessageIfTimeWorkStarted()
    {
        return
            "\n\u2705 Доброе утро! Чат открыт, готовы ответить на ваши вопросы \u2764\ufe0f";
    }
    
}