using Newtonsoft.Json.Linq;
using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectNonWorkingTime : SandBoxHelpers, IExecutable<bool>
{
    private bool _isAdminOrManager;
    private bool _isWorkTime;
    private static bool _isFirstSyncTime = true;

    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);

        if (_isFirstSyncTime)
            SyncWithServerTime().GetAwaiter().GetResult();

        AccountDb = Repository.Accounts.GetById(Update.Message.From.Id).Result;
        _isAdminOrManager = IfThisUserIsManager(Update.Message.From.Id, Update.Message.Chat.Id).Result;
        _isWorkTime = IsWorkTime();

        if (IfCanBeSendedMessage()) return Task.FromResult(true);
        
        BotClient.DeleteMessageAsync(chatId: Update.Message.Chat.Id, messageId: Update.Message.MessageId);

        return Task.FromResult(true);
    }
    
    
    private bool IfCanBeSendedMessage()
    {
        if (_isWorkTime)
            return _isWorkTime;

        if (_isAdminOrManager)
            return _isAdminOrManager;

        return false;
    }

    private bool IsWorkTime()
    {
        return WorkTimeChatTimer.IsWorkTime();
    }
    
    private string BuildMessageAsyncTime(DateTime serverTime, DateTime currentTime)
    {
        return $"⚠️ Внимание! Обнаружено расcинхронизация по времени:\n\n" +
               $"\u23f0 Текущее время на сервере: {currentTime:yyyy-MM-dd HH:mm}\n\n" +
               $"\u23f0 Фактическое время по интернету: {serverTime:yyyy-MM-dd HH:mm}\n\n" +
               $"⚠️Это очень сильно может повлиять на работу бота";
    }
    
    private string BuildMessageUnsuccessCheck(DateTime serverTime)
    {
        return $"⚠️ Внимание! Не удалось выполнить проверку времени через интернет:\n\n" +
               $"\u23f0 Серверное время: {serverTime:yyyy-MM-dd HH:mm}\n\n" +
               $"⚠️ Если время не совпадает с фактическим, это может повлиять на работу бота";
    }
    
    private async Task SyncWithServerTime()
    {
        _isFirstSyncTime = false;
        try
        {
            using var httpClient = new HttpClient();
            
            var response = await httpClient.GetStringAsync("https://www.timeapi.io/api/Time/current/ip?ipAddress=85.236.170.146");
            var json = JObject.Parse(response);
            var serverDateTime = DateTime.Parse(json["dateTime"]?.ToString() ?? string.Empty);

            var currentTime = DateTime.Now;
            var serverTime = new TimeOnly(serverDateTime.Hour, serverDateTime.Minute);
            var localTime = new TimeOnly(currentTime.Hour, currentTime.Minute);
            
            if (serverDateTime.Date != currentTime.Date || serverTime != localTime)
            {
                foreach (var account in Repository.Accounts.GetManagers().Result)
                {
                    try
                    {
                        await BotClient.SendTextMessageAsync(chatId: account.IdTelegram,
                            text: BuildMessageAsyncTime(serverDateTime, currentTime));
                    }
                    catch (Exception e)
                    {
                        if(e is ApiRequestException)
                            continue;
                    }
                }
            }
        }
        catch 
        {
            
            foreach (var account in Repository.Accounts.GetManagers().Result)
            {
                await BotClient.SendTextMessageAsync(chatId: account.IdTelegram, 
                    text: BuildMessageUnsuccessCheck(DateTime.Now));
            }
        }
    }
}