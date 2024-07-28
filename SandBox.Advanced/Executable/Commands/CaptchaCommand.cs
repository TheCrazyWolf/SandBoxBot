using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Executable.Commands.CaptchaTypes;
using SandBox.Advanced.Executable.Commands.CaptchaTypes.Common;
using SandBox.Advanced.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Commands;

public class CaptchaCommand(SandBoxRepository repository, ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/captcha";

    private readonly IList<CaptchaBase> _captchas = new List<CaptchaBase>
    {
        new CaptchaMath(), new CaptchaEmoji(), new CaptchaImNotBot()
    };

    public override async void Execute(Message message)
    {
        if (message.From is null)
            return;

        var account = await repository.Accounts.GetByIdAsync(message.From.Id);

        if ((account != null) && account.IsTrustedProfile())
        {
            TrySendMessage(idChat: message.Chat.Id,
                message: BuildMessageIfNoNeedCaptcha(),
                new LinkedList<InlineKeyboardButton>());

            repository.Accounts.UpdateApprovedAsync(account);
            return;
        }

        if (await repository.Captchas.ContainsCaptchasAsync(message.From.Id))
        {
            TrySendMessage(idChat: message.Chat.Id,
                message: BuildIfContainsNonCompletedCaptcha(),
                new LinkedList<InlineKeyboardButton>());
            return;
        }

        var captchaResult = _captchas.OrderBy(_ => new Random().Next())
            .First().Generate(message.From.Id);
        
        await repository.Captchas.NewCaptchaAsync(captchaResult.CatchaToDb);
        
        TrySendMessage(idChat: message.Chat.Id,
            message: BuildMsgWithCaptcha(captchaResult),
            GenerateKeyboard(captchaResult));
    }
    

    private void TrySendMessage(long idChat, string message, IReadOnlyCollection<InlineKeyboardButton> keyboardButtons)
    {
        try
        {
            botClient.SendTextMessageAsync(chatId: idChat,
                text: message,
                replyMarkup: new InlineKeyboardMarkup(keyboardButtons),
                disableNotification: true);
        }
        catch 
        {
            // ignored
        }
    }

    private string BuildMessageIfNoNeedCaptcha()
    {
        return
            "\u2705 Вам не требуется проходить проверку на бота, мы Вам доверяем";
    }
    
    private string BuildIfContainsNonCompletedCaptcha()
    {
        return
            "\u26a0\ufe0f У Вас есть ранее нерешенные каптчи";
    }

    private string BuildMsgWithCaptcha(CaptchaResult captchaResult)
    {
        return
            $"\u2734\ufe0f Подтвердите проверку, что ваша учетная запись используется действительно живым человеком " +
            $"пройдя простую капчу: \n\n{captchaResult.Message}\n\nЭту капчу необходимо решить до: {captchaResult.CatchaToDb.DateTimeExpired} и " +
            $"{captchaResult.CatchaToDb.AttemptsRemain} попытки";
    }

    private IReadOnlyCollection<InlineKeyboardButton> GenerateKeyboard(CaptchaResult captchaResult)
    {
        return captchaResult.Answers.Select(answer =>
            InlineKeyboardButton.WithCallbackData($"{answer}", 
                $"captcha {captchaResult.CatchaToDb.Id} {answer} ")).ToList();
    }
}
