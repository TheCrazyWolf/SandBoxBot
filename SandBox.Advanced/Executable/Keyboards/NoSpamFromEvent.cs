using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Executable.Common;
using SandBox.Models.Events;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Keyboards;

public class NoSpamFromEvent : SandBoxHelpers, IExecutable<bool>
{
    private EventContent? _eventContent;
    
    public Task<bool> Execute()
    {
        var words = Update.CallbackQuery?.Data?.Split(' ').Skip(1).ToArray();
        
        if(words is null)
            return Task.FromResult(false);
        
        _eventContent = Repository.Contents.GetById(Convert.ToInt64(words[0])).Result;

        if (_eventContent is null)
            return Task.FromResult(false);

        AccountDb = Repository.Accounts.GetById(Convert.ToInt64(_eventContent.IdTelegram)).Result;
        
        if (AccountDb is not null)
        {
            AccountDb.IsSpamer = false;
            AccountDb.IsAprroved = true;
            Repository.Accounts.Update(AccountDb);
        }
        
        _eventContent.IsSpam = false;
        Repository.Contents.Update(_eventContent);
        
        Proccess();
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
    

    private string BuildNotifyMessage()
    {
        return
            $"\u2705 Принятые действия по событию № {_eventContent?.Id}: Сообщение отмечено как не спам";
    }
    
}