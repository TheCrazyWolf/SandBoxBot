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
        var words = callbackQuery.Data?.Split(' ').Skip(1).ToArray();
        
        if (words is null)
            return;

        var captha = repository.Captchas.GetById(Convert.ToInt64(words[0])).Result;

        if (captha is null)
            return;
            
        if (captha.AttemptsRemain <= 0 || captha.DateTimeExpired <= DateTime.Now ||
            captha.IdTelegram != callbackQuery.From.Id)
        {
            botClient.AnswerCallbackQueryAsync(callbackQuery.Id, BuildErrorCaptcha(), true);
            repository.Captchas.UpdateDecrementAttemp(captha);
            if (callbackQuery.Message != null)
                botClient.DeleteMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                    messageId: callbackQuery.Message.MessageId);
            return;
        }

        if (words[1] != captha.Content)
        {
            botClient.AnswerCallbackQueryAsync(callbackQuery.Id, BuildWrongCaptcha(), true);
            repository.Captchas.UpdateDecrementAttemp(captha);
            return;
        }

        var account = repository.Accounts.GetByIdAsync(Convert.ToInt64(captha.IdTelegram)).Result;
        
        if(account is null)
            return;
        
        repository.Accounts.UpdateApproved(account);
        botClient.AnswerCallbackQueryAsync(callbackQuery.Id, BuildSuccessCaptcha(), true);
        
        if (callbackQuery.Message != null)
            botClient.DeleteMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                messageId: callbackQuery.Message.MessageId);
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