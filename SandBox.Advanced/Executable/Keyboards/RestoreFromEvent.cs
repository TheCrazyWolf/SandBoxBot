using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Models.Events;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Keyboards;

public class RestoreFromEvent(SandBoxRepository repository, ITelegramBotClient botClient) : CallQuery
{
    public override string Name { get; set; } = "restoremsg"; // FORMAT: restoremsg ID_CHAT ID_USER ID_EVENT

    public override async void Execute(CallbackQuery callbackQuery)
    {
        var words = TryGetArrayFromCallBack(callbackQuery);

        if (words is null) return;
        var @event = repository.Contents.GetByIdAsync(Convert.ToInt64(words[2])).Result;

        if (@event is null) return;

        var account = repository.Accounts.GetByIdAsync(Convert.ToInt64(@event.IdTelegram)).Result;
        if (account is null) return;

        var memberChat = await repository.MembersInChat.GetByIdAsync(idChat: Convert.ToInt64(@event.ChatId),
            idTelegram: Convert.ToInt64(@event.IdTelegram));

        if (memberChat is null) return;

        //repository.Accounts.UpdateApprovedAsync(account);
        repository.MembersInChat.UpdateAprrovedAsync(memberChat);
        repository.Contents.UpdateNoSpamAsync(@event);


        var isSuccessSend = await TrySendMessage(chatId: Convert.ToInt64(@event.ChatId),
            message: BuildRestoredMessage(account, @event));

        TryAnswerOnCallBack(callbackQuery.Id, isSuccessSend);

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

    private async Task<bool> TrySendMessage(long chatId, string message)
    {
        try
        {
            await botClient.SendTextMessageAsync(chatId: chatId,
                text: message,
                disableNotification: true);
        }
        catch
        {
            return false;
        }

        return true;
    }

    private async void TryAnswerOnCallBack(string callbackQueryId, bool success)
    {
        try
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId,
                success ? BuildSuccessMessage() : BuildErrorMessage(), true);
        }
        catch
        {
            // ingored
        }
    }

    private string BuildSuccessMessage()
    {
        return
            $"\u2705 Сообщение восстановлено в чат";
    }

    private string BuildErrorMessage()
    {
        return
            $"\u274c Не удалось отправить сообщение в чат";
    }

    private string BuildRestoredMessage(Account sender, EventContent content)
    {
        return
            $"@{sender.UserName ?? $"{sender.FirstName} {sender.LastName}"}: {content.Content}";
    }
}