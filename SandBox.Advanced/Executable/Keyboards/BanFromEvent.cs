using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Models.Events;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Keyboards;

public class BanFromEvent(
    ITelegramBotClient botClient,
    CallbackQuery callbackQuery,
    SandBoxRepository repository) : IExecutable<bool>
{
    private EventContent? _eventContent;

    public Task<bool> Execute()
    {
        var words = callbackQuery.Data?.Split(' ').Skip(1).ToArray();
        
        if(words is null)
            return Task.FromResult(false);
        
        _eventContent = repository.Contents.GetById(Convert.ToInt64(words[0])).Result;

        if (_eventContent is null)
            return Task.FromResult(false);

        Proccess();
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

    private string BuildNotifyMessage()
    {
        return
            $"\u2705 Принятые действия по событию № {_eventContent?.Id}: Пользователь заблокирован";
    }
    
}