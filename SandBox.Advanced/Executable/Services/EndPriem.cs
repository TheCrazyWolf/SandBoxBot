using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Services;

public class EndPriem(SandBoxRepository repository, ITelegramBotClient botClient) : IService
{
    private static readonly DateTime endPriem = new(2024, 08, 15, 16, 00, 00);
    public static Message? _messageCounter = default!;
    public static long ChatId { get; set; }
    public async Task Execute()
    {
        while (true)
        {
            if (botClient is null)
                throw new Exception("botClient is null");

            if (_messageCounter is null || ChatId is 0)
            {
                await Task.Delay(1000); // Проверка каждую секунду
                continue;
            }

            var now = DateTime.Now;

            if (now >= endPriem)
            {
                await TryEditMessage(BuildNotifyMessage());
                await Task.Delay(1000); // Проверка каждую секунду
                return;
            }

            await TryEditMessage(BuildNotifyMessage(endPriem - now));
            await Task.Delay(1000); // Проверка каждую секунду
        }
    }

    private string BuildNotifyMessage(TimeSpan timeSpan)
    {
        return
            $"\u23f0 До конца приёмной кампании {timeSpan.TotalDays} д. {timeSpan.Hours} ч. {timeSpan.Minutes} м. {timeSpan.Seconds} с.";
    }
    
    private string BuildNotifyMessage()
    {
        return
            $"\u23f0 Приемная кампания завершилась :)";
    }

    private async Task TryEditMessage(string message)
    {
        try
        {
            if (_messageCounter != null)
                await botClient.EditMessageTextAsync(chatId: ChatId, _messageCounter.MessageId, message);
        }
        catch
        {
            // ignored
        }
    }
}