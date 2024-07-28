using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Keyboards;

public class NoSpamFromEvent(SandBoxRepository repository, ITelegramBotClient botClient) : CallQuery
{
    public override string Name { get; set; } = "nospam"; // FORMAT: nospam ID_CHAT ID_USER ID_EVENT

    public override void Execute(CallbackQuery callbackQuery)
    {
        var words = TryGetArrayFromCallBack(callbackQuery);

        if (words is null) return;
        
        var @event = repository.Contents.GetByIdAsync(Convert.ToInt64(words[2])).Result;

        if (@event is null) return;

        var account = repository.Accounts.GetByIdAsync(Convert.ToInt64(@event.IdTelegram)).Result;
        if (account is null) return;

        repository.Accounts.UpdateApprovedAsync(account);
        repository.Contents.UpdateNoSpamAsync(@event);

        TryAnswerOnCallBack(callbackQuery.Id);

        if (callbackQuery.Message != null)
            TryRemoveMessageAfterCallback(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
    }
    
    private string[]? TryGetArrayFromCallBack(CallbackQuery callbackQuery)
    {
        try
        {
            return callbackQuery.Data?.Split(' ').Skip(1).ToArray();
        }
        catch
        {
            return null;
        }
    }

    private async void TryRemoveMessageAfterCallback(long chatId, int messageId)
    {
        try
        {
            await botClient.DeleteMessageAsync(chatId: chatId, messageId: messageId);
        }
        catch
        {
            //ignored
        }
    }

    private async void TryAnswerOnCallBack(string callbackQueryId)
    {
        try
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId, BuildNotifyMessage(), true);
        }
        catch
        {
            // ingored
        }
    }

    private string BuildNotifyMessage()
    {
        return
            $"\u2705 Отметили, что это не спам. Спасибо";
    }
}