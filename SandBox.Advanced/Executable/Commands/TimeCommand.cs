using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class TimeCommand(ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/time";

    private readonly TimeServer _timeServer = new();
    public override void Execute(Message message)
    {
        if (message.From is null)
            return;

        var result = GetServerTimeNow().Result;
        var localTime = DateTime.Now;

        if (result.Item2 && !IsSyncTime(result.Item1, localTime))
        {
            SendMessage(idChat: message.Chat.Id, message: _timeServer.
                BuildMessageAsyncTime(result.Item1, localTime));
            return;
        }
        else if(!result.Item2)
        {
            SendMessage(idChat: message.Chat.Id, message: _timeServer.
                BuildMessageUnsuccessCheck(localTime));
            return;
        }

        SendMessage(idChat: message.Chat.Id, message: _timeServer
            .BuildMessageSyncTime(result.Item1, localTime));
    }
    
    private void SendMessage(long idChat, string message)
    {
        botClient.SendTextMessageAsync(chatId: idChat,
            text: message,
            disableNotification: true);
    }
    
    private async Task<(DateTime, bool)> GetServerTimeNow()
    {
        try
        {
            var resultTime = await _timeServer.GetServerTime();
            return (resultTime, true);
        }
        catch (Exception e)
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
    
}