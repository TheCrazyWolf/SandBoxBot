using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Keyboards;

public class NoSpamFromEvent(SandBoxRepository repository, ITelegramBotClient botClient) : CallQuery
{
    public override string Name { get; set; } = "spamnospam";
    public override void Execute(CallbackQuery callbackQuery)
    {
        var words = callbackQuery.Data?.Split(' ').Skip(1).ToArray();
        
        if (words is null) return;
        var @event = repository.Contents.GetById(Convert.ToInt64(words[0])).Result;
        
        if (@event is null) return;

        var account = repository.Accounts.GetByIdAsync(Convert.ToInt64(@event.IdTelegram)).Result;
        if (account is null) return;

        repository.Accounts.UpdateApprovedAsync(account);
        repository.Contents.UpdateNoSpam(@event);

        botClient.AnswerCallbackQueryAsync(callbackQuery.Id, BuildNotifyMessage(@event.Id), true);
    }
    
    private string BuildNotifyMessage(long id)
    {
        return
            $"\u2705 Принятые действия по событию № {id}: \n\nСообщение отмечено как не спам";
    }
    
}