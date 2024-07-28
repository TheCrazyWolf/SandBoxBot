using SandBox.Advanced.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Keyboards;

public class KickKeyboard(ITelegramBotClient botClient) : CallQuery
{
    public override string Name { get; set; } = "kick"; // FORMAT: KICK ID_CHAT ID_USER

    public override async void Execute(CallbackQuery callbackQuery)
    {
        var words = TryGetArrayFromCallBack(callbackQuery);

        if (words is null) return;

        var resultOfBan = await TryKickFromChat(Convert.ToInt64(words[0]), Convert.ToInt64(words[1]));
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
    
    private async Task<bool> TryKickFromChat(long idChat, long idTelegram)
    {
        try
        {
            await botClient.UnbanChatMemberAsync(chatId: idChat,
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
            // ignored
        }
    }

    private string BuildSuccessMessage()
    {
        return
            $"\u2705 Пользователь выгнан из это чата";
    }

    private string BuildErrorMessage()
    {
        return
            $"\u274c Не удалось изгнать пользователя";
    }
}