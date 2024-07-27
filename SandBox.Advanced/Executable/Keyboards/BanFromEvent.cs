using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Keyboards;

public class BanFromEvent(SandBoxRepository repository, ITelegramBotClient botClient) : CallQuery
{
    public override string Name { get; set; } = "spamban";
    public override void Execute(CallbackQuery callbackQuery)
    {
        var words = callbackQuery.Data?.Split(' ').Skip(1).ToArray();
        
        if (words is null)
            return;

        var @event = repository.Events.GetById(Convert.ToInt64(words[0])).Result;
        
        if (@event is null)
            return;
        
        var account = repository.Accounts.GetByIdAsync(Convert.ToInt64(@event.IdTelegram)).Result;

        if (account is null) return;
        
        repository.Accounts.UpdateToSpamer(account);

        if (@event.ChatId != null)
            botClient.BanChatMemberAsync(chatId: @event.ChatId,
                userId: Convert.ToInt64(@event.IdTelegram));
            
        botClient.AnswerCallbackQueryAsync(callbackQuery.Id, BuildNotifyMessage(@event.Id), true);
    }

    private void SendMessageOfExecuted(long idChat, string message)
    {
        botClient.SendTextMessageAsync(chatId: idChat,
            text: message,
            disableNotification: true);
    }

    private string BuildNotifyMessage(long id)
    {
        return
            $"\u2705 Принятые действия по событию № {id}: \n\nСообщение отмечено как не спам, восстановлено сообщение в беседу";
    }
    
}