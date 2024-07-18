using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using SandBox.Models.Events;
using SandBox.Models.Telegram;
using Telegram.Bot;

namespace SandBox.Advanced.Executable.Keyboards;

public class RestoreFromEvent: SandBoxHelpers, IExecutable<bool>
{
    private EventContent? _eventContent;
    private Account? _senderOfEvent;
    
    public Task<bool> Execute()
    {
        var words = Update.CallbackQuery?.Data?.Split(' ').Skip(1).ToArray();
        
        if(words is null)
            return Task.FromResult(false);
        
        _eventContent = Repository.Contents.GetById(Convert.ToInt64(words[0])).Result;

        if (_eventContent is null)
            return Task.FromResult(false);
        
        _eventContent.IsSpam = false;
        Repository.Contents.Update(_eventContent);
        
        AccountDb = Repository.Accounts.GetById(Convert.ToInt64(_eventContent.IdTelegram)).Result;
        
        if (AccountDb is not null)
        {
            AccountDb.IsSpamer = false;
            Repository.Accounts.Update(AccountDb);
        }
        
        _senderOfEvent = Repository.Accounts.GetById(Convert.ToInt64(_eventContent?.IdTelegram)).Result;
        
        Proccess();
        SendMessageRestorable();
        SendMessageOfExecuted();
        return Task.FromResult(true);
    }
    
    private void Proccess()
    {
        BotClient.BanChatMemberAsync(chatId: _eventContent!.ChatId!,
            userId: Convert.ToInt64(_eventContent.IdTelegram));
    }
    
    private void SendMessageOfExecuted()
    {
        var message = BuildNotifyMessage();

        BotClient.SendTextMessageAsync(chatId: Update.CallbackQuery?.From.Id!,
            text: message,
            disableNotification: true);
    }
    
    private void SendMessageRestorable()
    {
        var message = BuildRestoredMessage();

        BotClient.SendTextMessageAsync(chatId: _eventContent!.ChatId!,
            text: message,
            disableNotification: true);
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