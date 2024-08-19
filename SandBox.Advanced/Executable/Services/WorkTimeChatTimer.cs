using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Services;

public class WorkTimeChatTimer(SandBoxRepository repository, ITelegramBotClient botClient, long idChat) : IService
{
    private static readonly TimeOnly StartWorkTime = new(08, 00, 00);
    private static readonly TimeOnly EndWorkTimeMonday = new(16, 00, 00);
    private static readonly TimeOnly EndWorkTimeDefault = new(16, 00, 00);

    private static DateTime _lastSendStartMessageDay;
    private static DateTime _lastSendEndMessageDay;
    
    public async Task Execute()
    {
        while (true)
        {
            if (botClient is null)
                throw new Exception("botClient is null");

            var now = DateTime.Now;
            if (ShouldSendStartMessage(now))
            {
                await botClient.SendTextMessageAsync(chatId: idChat,
                    text: BuildMessageIfTimeWorkStarted());
                _lastSendStartMessageDay = now;
                await botClient.SetChatPermissionsAsync(chatId: idChat, GetPermissions(true));
                NotifyManagers(BuildNotifyMessage(true));
            }

            if (ShouldSendEndMessage(now))
            {
                await botClient.SendTextMessageAsync(chatId: idChat,
                    text: BuildMessageIfTimeWorkEnd());
                _lastSendEndMessageDay = now;
                await botClient.SetChatPermissionsAsync(chatId: idChat, GetPermissions(false));
                NotifyManagers(BuildNotifyMessage(false));
            }

            await Task.Delay(1000); // Проверка каждую секунду
        }
    }

    private string BuildNotifyMessage(bool allowToSendMsg)
    {
        string allowed = allowToSendMsg ? "Да" : "Нет";
        return
            $"\u2705 Настройки чата: {idChat} обновлены в связи с началом/конца рабочего дня: \n\nПользователи могут писать сообщения: {allowed}";
    }

    private void NotifyManagers(string message)
    {
        foreach (var id in repository.Accounts.GetManagersAsync().Result)
        {
            try
            {
                botClient.SendTextMessageAsync(chatId: id.IdTelegram,
                    text: message,
                    disableNotification: true);
            }
            catch
            {
                // ignored
            }
        }
    }

    private ChatPermissions GetPermissions(bool isActive)
    {
        return new ChatPermissions
        {
            CanSendAudios = isActive,
            CanSendDocuments = isActive,
            CanSendPhotos = isActive,
            CanSendPolls = isActive,
            CanSendVoiceNotes = isActive,
            CanSendMessages = isActive,
            CanSendVideoNotes = isActive,
            CanSendVideos = isActive,
            CanSendOtherMessages = isActive,
        };
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
            "\n\u2705 Но получить ответы на вопросы Вы можете в рабочее время: \n\n\u23f0 ПН-ПТ с 8.00 до 16.00 (Самарское)";
    }

    private static string BuildMessageIfTimeWorkStarted()
    {
        return
            "\n\u2705 Доброе утро! Чат открыт, готовы ответить на ваши вопросы \u2764\ufe0f";
    }
}