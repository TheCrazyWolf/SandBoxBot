using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Models.Events;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Keyboards;

public class RestoreFromEvent(
    ITelegramBotClient botClient,
    CallbackQuery callbackQuery,
    SandBoxRepository repository) : IExecutable<bool>
{
    private EventContent? _eventContent;
    private Account? _senderOfEvent;
    
    public Task<bool> Execute()
    {
        var words = callbackQuery.Data?.Split(' ').Skip(1).ToArray();
        
        if(words is null)
            return Task.FromResult(false);
        
        _eventContent = repository.Contents.GetById(Convert.ToInt64(words[0])).Result;

        if (_eventContent is null)
            return Task.FromResult(false);
        
        _eventContent.IsSpam = false;
        repository.Contents.Update(_eventContent);
        
        _senderOfEvent = repository.Accounts.GetById(Convert.ToInt64(_eventContent?.IdTelegram)).Result;
        
        Proccess();
        SendMessageRestorable();
        SendMessageOfExecuted();
        return Task.FromResult(true);
    }
    
    private Task Proccess()
    {
        botClient.BanChatMemberAsync(chatId: _eventContent!.ChatId!,
            userId: Convert.ToInt64(_eventContent.IdTelegram));
        return Task.CompletedTask;
    }
    
    private Task SendMessageOfExecuted()
    {
        var message = BuildNotifyMessage();

        botClient.SendTextMessageAsync(chatId: callbackQuery.From.Id,
            text: message,
            disableNotification: true);
        
        return Task.CompletedTask;
    }
    
    private Task SendMessageRestorable()
    {
        var message = BuildRestoredMessage();

        botClient.SendTextMessageAsync(chatId: _eventContent!.ChatId!,
            text: message,
            disableNotification: true);
        
        return Task.CompletedTask;
    }

    private string BuildNotifyMessage()
    {
        return
            $"\u2705 Принятые действия по событию № {_eventContent?.Id}: Сообщение отмечено как не спам, восстановлено сообщение в беседу";
    }
    
    private string BuildRestoredMessage()
    {
        return
            $"\ud83d\uddd3 (Восстановлено) @{_senderOfEvent?.UserName}: {_eventContent?.Content}";
    }
    
}