using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Keyboards;

public class BanFromChat(SandBoxRepository repository, ITelegramBotClient botClient) : CallQuery
{
    public override string Name { get; set; } = "ban";

    public override void Execute(CallbackQuery callbackQuery)
    {
        var words = callbackQuery.Data?.Split(' ').Skip(1).ToArray();
        
        if (words is null)
            return;

        var @event = repository.Events.GetById(Convert.ToInt64(words[0])).Result;
        
        if (@event is null)
            return;
        
        var account = repository.Accounts.GetById(Convert.ToInt64(@event.IdTelegram)).Result;

        if (account is null) return;
        
        repository.Accounts.UpdateToSpamer(account);

        if (@event.ChatId != null)
            botClient.BanChatMemberAsync(chatId: @event.ChatId,
                userId: Convert.ToInt64(@event.IdTelegram));
            
        botClient.AnswerCallbackQueryAsync(callbackQuery.Id, BuildNotifyMessage(), true);
    }

    private string BuildNotifyMessage()
    {
        return
            $"\u2705 Принятые действия: \n\nПользователь заблокирован";
    }
    
}