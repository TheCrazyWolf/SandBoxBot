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
            TrySendMessage(idChat: message.Chat.Id, message: _timeServer.
                BuildMessageAsyncTime(result.Item1, localTime));
            return;
        }
        else if(!result.Item2)
        {
            TrySendMessage(idChat: message.Chat.Id, message: _timeServer.
                BuildMessageUnsuccessCheck(localTime));
            return;
        }

        TrySendMessage(idChat: message.Chat.Id, message: _timeServer
            .BuildMessageSyncTime(result.Item1, localTime));
    }
    
    private void TrySendMessage(long idChat, string message)
    {
        try
        {
            botClient.SendTextMessageAsync(chatId: idChat,
                text: message,
                disableNotification: true);
        }
        catch
        {
            // ignored
        }
    }
    
    private async Task<(DateTime, bool)> GetServerTimeNow()
    {
        try
        {
            var resultTime = await _timeServer.GetServerTime();
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
    
}