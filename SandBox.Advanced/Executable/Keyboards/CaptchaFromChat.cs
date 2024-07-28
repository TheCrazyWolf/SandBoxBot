using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Keyboards;

public class CaptchaFromChat(SandBoxRepository repository, ITelegramBotClient botClient) : CallQuery
{
    public override string Name { get; set; } = "captcha";

    public override void Execute(CallbackQuery callbackQuery)
    {
        var words = TryGetArrayFromCallBack(callbackQuery);

        if (words is null) return;

        var captcha = repository.Captchas.GetById(Convert.ToInt64(words[0])).Result;

        if (captcha is null)
            return;

        if (captcha.AttemptsRemain <= 0 || captcha.DateTimeExpired <= DateTime.Now ||
            captcha.IdTelegram != callbackQuery.From.Id)
        {
            botClient.AnswerCallbackQueryAsync(callbackQuery.Id, BuildErrorCaptcha(), true);
            repository.Captchas.UpdateDecrementAttemp(captcha);
            if (callbackQuery.Message != null)
                botClient.DeleteMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                    messageId: callbackQuery.Message.MessageId);
            return;
        }

        if (words[1] != captcha.Content)
        {
            botClient.AnswerCallbackQueryAsync(callbackQuery.Id, BuildWrongCaptcha(), true);
            repository.Captchas.UpdateDecrementAttemp(captcha);
            return;
        }

        var account = repository.Accounts.GetByIdAsync(Convert.ToInt64(captcha.IdTelegram)).Result;

        if (account is null)
            return;

        repository.Accounts.UpdateApprovedAsync(account);
        botClient.AnswerCallbackQueryAsync(callbackQuery.Id, BuildSuccessCaptcha(), true);

        if (callbackQuery.Message != null)
            botClient.DeleteMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                messageId: callbackQuery.Message.MessageId);
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