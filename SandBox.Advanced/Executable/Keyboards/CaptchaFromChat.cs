using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Keyboards;

public class CaptchaFromChat(SandBoxRepository repository, ITelegramBotClient botClient) : CallQuery
{
    public override string Name { get; set; } = "captcha";

    public override async void Execute(CallbackQuery callbackQuery)
    {
        var words = TryGetArrayFromCallBack(callbackQuery);

        if (words is null) return;

        var captcha = await repository.Captchas.GetByIdAsync(Convert.ToInt64(words[0]));

        if (captcha is null) return;

        if (captcha.AttemptsRemain <= 0 || captcha.DateTimeExpired <= DateTime.Now ||
            captcha.IdTelegram != callbackQuery.From.Id)
        {
            TryAnswerOnCallBack(callbackQuery.Id, BuildErrorCaptcha());
            repository.Captchas.UpdateDecrementAttempAsync(captcha);
            
            if (callbackQuery.Message != null) 
                TryRemoveMessageAfterCallback(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
            return;
        }

        if (words[1] != captcha.Content)
        {
            TryAnswerOnCallBack(callbackQuery.Id, BuildWrongCaptcha());
            repository.Captchas.UpdateDecrementAttempAsync(captcha);
            return;
        }

        var account = await repository.Accounts.GetByIdAsync(Convert.ToInt64(captcha.IdTelegram));

        if (account is null) return;

        repository.Accounts.UpdateApprovedAsync(account);
        TryAnswerOnCallBack(callbackQuery.Id, BuildSuccessCaptcha());

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
    
    private async void TryAnswerOnCallBack(string callbackQueryId, string message)
    {
        try
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId,message, true);
        }
        catch
        {
            // ingored
        }
    }

    private string BuildErrorCaptcha()
    {
        return
            $"\u26a0\ufe0f Ошибка во время прохождения капчи. \n\nВозможно: ваши попытки закончились, срок её прохождения закончился или проходите сгенерированную капчу не для Вас";
    }

    private string BuildWrongCaptcha()
    {
        return
            $"\u26a0\ufe0f Ответ неправильный. \n\nКоличество попыток ограничено";
    }

    private string BuildSuccessCaptcha()
    {
        return
            "\u2705 Вы подтвердили, что вы не бот. Спасибо";
    }
}