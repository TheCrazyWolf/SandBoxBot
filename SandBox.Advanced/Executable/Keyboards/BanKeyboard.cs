using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Keyboards;

public class BanKeyboard(SandBoxRepository repository, ITelegramBotClient botClient) : CallQuery
{
    public override string Name { get; set; } = "ban"; // FORMAT: BAN ID_CHAT ID_USER

    public override async void Execute(CallbackQuery callbackQuery)
    {
        var words = TryGetArrayFromCallBack(callbackQuery);

        if (words is null) return;

        var resultOfBan = await TryBanFromChat(Convert.ToInt64(words[0]), Convert.ToInt64(words[1]));
        TryAnswerOnCallBack(callbackQuery.Id, resultOfBan);
        
        if (callbackQuery.Message != null) 
            TryRemoveMessageAfterCallback(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
    }
    
    private string[]? TryGetArrayFromCallBack(CallbackQuery callbackQuery)
    {
        try
        {
            return callbackQuery.Data?.Split(' ').Skip(1).ToArray();
        }
        catch (Exception e)
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

    private async Task<bool> TryBanFromChat(long idChat, long idTelegram)
    {
        try
        {
            await botClient.BanChatMemberAsync(chatId: idChat,
                userId: idTelegram);
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

    private void SendMessageOfExecuted(long idChat, string message)
    {
        botClient.SendTextMessageAsync(chatId: idChat,
            text: message,
            disableNotification: true);
    }

    private string BuildSuccessMessage()
    {
        return
            $"\u2705 Пользователь заблокирован в этом чате";
    }

    private string BuildErrorMessage()
    {
        return
            $"\u274c Не удалось заблокировать пользователя";
    }
}