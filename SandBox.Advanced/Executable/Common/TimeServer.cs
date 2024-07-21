using Newtonsoft.Json.Linq;

namespace SandBox.Advanced.Executable.Common;

public class TimeServer
{
    public string BuildMessageAsyncTime(DateTime serverTime, DateTime currentTime)
    {
        return $"⚠️ Внимание! Обнаружено расcинхронизация по времени:\n\n" +
               $"\u23f0 Текущее время на сервере: {currentTime:yyyy-MM-dd HH:mm}\n\n" +
               $"\u23f0 Фактическое время по интернету: {serverTime:yyyy-MM-dd HH:mm}\n\n" +
               $"⚠️Это очень сильно может повлиять на работу бота";
    }
    
    public string BuildMessageUnsuccessCheck(DateTime serverTime)
    {
        return $"⚠️ Внимание! Не удалось выполнить проверку времени через интернет:\n\n" +
               $"\u23f0 Серверное время: {serverTime:yyyy-MM-dd HH:mm}\n\n" +
               $"⚠️ Если время не совпадает с фактическим, это может повлиять на работу бота";
    }
    
    public string BuildMessageSyncTime(DateTime serverTime, DateTime currentTime)
    {
        return $"\u23f0 Текущее время на сервере: {currentTime:yyyy-MM-dd HH:mm}\n\n" +
               $"\u23f0 Фактическое время по интернету: {serverTime:yyyy-MM-dd HH:mm}\n\n";
    }

    public async Task<DateTime> GetServerTime()
    {
        using var httpClient = new HttpClient();
            
        var response = await httpClient.GetStringAsync("https://www.timeapi.io/api/Time/current/ip?ipAddress=85.236.170.146");
        var json = JObject.Parse(response);
        var serverDateTime = DateTime.Parse(json["dateTime"]?.ToString() ?? string.Empty);

        return serverDateTime;
    }
    
}