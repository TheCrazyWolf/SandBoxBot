using SandBox.Advanced.Database;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectAsyncServerTime(SandBoxRepository repository, ITelegramBotClient botClient) : TimeServer, IAnalyzer
{
    private static bool _isFirstExecute = true;

    public async void Execute(Message message)
    {
        if (!_isFirstExecute)
            return;

        var result = await GetServerTimeNow();
        var localTime = DateTime.Now;

        switch (result.Item2)
        {
            case true when !IsSyncTime(result.Item1, localTime):
                NotifyManagers(BuildMessageAsyncTime(result.Item1, localTime));
                break;
            case false:
                NotifyManagers(BuildMessageUnsuccessCheck(localTime));
                break;
        }
        
        _isFirstExecute = false;
    }

    private async Task<(DateTime, bool)> GetServerTimeNow()
    {
        try
        {
            var resultTime = await GetServerTime();
            return (resultTime, true);
        }
        catch
        {
            return (DateTime.Now, false);
        }
    }

    private bool IsSyncTime(DateTime serverDateTime, DateTime localDateTime)
    {
        var serverTime = new TimeOnly(serverDateTime.Hour, serverDateTime.Minute);
        var localTime = new TimeOnly(localDateTime.Hour, localDateTime.Minute);

        return (serverDateTime.Date == localDateTime.Date) && serverTime == localTime;
    }

    private async void NotifyManagers(string message)
    {
        foreach (var id in await repository.Accounts.GetManagersAsync())
        {
            try
            {
                _ = botClient.SendTextMessageAsync(chatId: id.IdTelegram,
                    text: message,
                    disableNotification: true);
            }
            catch
            {
                // ignored
            }
        }
    }
}