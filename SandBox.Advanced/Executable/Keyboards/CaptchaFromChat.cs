using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using SandBox.Models.Blackbox;
using Telegram.Bot;

namespace SandBox.Advanced.Executable.Keyboards;

public class CaptchaFromChat : SandBoxHelpers, IExecutable<bool>
{
    private Captcha? _captcha;

    public Task<bool> Execute()
    {
        if (Update.CallbackQuery is null)
            return Task.FromResult(false);

        var words = Update.CallbackQuery.Data?.Split(' ').Skip(1).ToArray();

        if (words is null)
            return Task.FromResult(false);

        _captcha = Repository.Captchas.GetById(Convert.ToInt64(words[0])).Result;

        if (_captcha is null || _captcha.AttemptsRemain == 0 || _captcha.DateTimeExpired <= DateTime.Now ||
            _captcha.IdTelegram != Update.CallbackQuery?.From.Id)
        {
            SendMessageOfExecuted(idChat: Convert.ToInt64(words[2]), message: BuildErrorCaptcha());
            return Task.FromResult(false);
        }

        if (words[1] != _captcha.Content)
        {
            ProccessWrongCaptcha();
            SendMessageOfExecuted(idChat: Convert.ToInt64(words[2]), message: BuildWrongCaptcha());
            return Task.FromResult(false);
        }

        ProccessRightCaptcha(Convert.ToInt64(_captcha.IdTelegram));
        SendMessageOfExecuted(idChat: Convert.ToInt64(words[2]), message: BuildSuccessCaptcha());
        return Task.FromResult(true);
    }

    private void ProccessWrongCaptcha()
    {
        if (_captcha is null)
            return;

        _captcha.AttemptsRemain--;
        Repository.Captchas.Update(_captcha);
    }

    private void ProccessRightCaptcha(long idTelegram)
    {
        ProccessWrongCaptcha();
        AccountDb = Repository.Accounts.GetById(idTelegram).Result;

        if (AccountDb is null)
            return;

        AccountDb.IsSpamer = false;
        AccountDb.IsAprroved = true;
        AccountDb.IsNeedToVerifyByCaptcha = false;
        AccountDb = Repository.Accounts.Update(AccountDb).Result;
    }

    private void SendMessageOfExecuted(long idChat, string message)
    {
        BotClient.SendTextMessageAsync(chatId: idChat,
            text: message,
            disableNotification: true);
    }

    private string BuildErrorCaptcha()
    {
        return
            $"\u26a0\ufe0f Ошибка поиска каптчи или срок действия истек";
    }

    private string BuildWrongCaptcha()
    {
        return
            $"\u26a0\ufe0f Ответ неправильный";
    }
    
    private string BuildSuccessCaptcha()
    {
        return
            "\u2705 Вы подтвердили, что вы не бот. Спасибо";
    }
}