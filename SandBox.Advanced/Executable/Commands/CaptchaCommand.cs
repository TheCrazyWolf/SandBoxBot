using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using SandBox.Models.Blackbox;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Commands;

public class CaptchaCommand : SandBoxHelpers, IExecutable<bool>
{
    private IDictionary<string, string> _emojies = new Dictionary<string, string>()
    {
        { "🍏", "🍎" }, { "🤡", "💩" }, { "☠️", "👺" }, { "😛", "🍎" },
        { "🤖", "🎃" }, { "😳️", "🤯" }, { "👾", "😇" }, { "💥", "⚡️" },
        { "💦", "❄️" }, { "⛈", "🌤" }, { "🥥", "🥝" }
    };

    private Captcha _captcha = new();

    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);

        AccountDb = Repository.Accounts.GetById(Update.Message.From.Id).Result;
        if (AccountDb is null)
            return Task.FromResult(false);

        if (CanBeOverrideRestriction(idTelegram: Update.Message.From.Id, idChat: Update.Message.Chat.Id).Result &&
            !AccountDb.IsSpamer)
        {
            SendCaptcha(idChat: Update.Message.Chat.Id, message: BuildErrorMessage(),
                new LinkedList<InlineKeyboardButton>());
            AccountDb.IsAprroved = true;
            Repository.Accounts.Update(AccountDb);
            return Task.FromResult(true);
        }

        if (Convert.ToBoolean(new Random().Next(0, 2)))
            ProccessingFirstCaptcha(idTelegram: Update.Message.From.Id, idChat: Update.Message.Chat.Id);
        else
            ProccessingSecondCaptcha(idTelegram: Update.Message.From.Id, idChat: Update.Message.Chat.Id);


        return Task.FromResult(false);
    }

    private void ProccessingFirstCaptcha(long idTelegram, long idChat)
    {
        var first = new Random().Next(0, 15);
        var second = new Random().Next(0, 15);
        var summ = first + second;

        CreateCaptchToDb(idTelegram: idTelegram, content: summ.ToString());

        SendCaptcha(idChat: idChat,
            message: BuildMessesWithCaptcha(
                $"Решите математический пример: \n\nВыберите сумму числа {first} и {second}"),
            keyboardButtons: GenerateKeyboardMathematic(idChat));
    }

    private void ProccessingSecondCaptcha(long idTelegram, long idChat)
    {
        var rnd = new Random();
        var emoji = _emojies.Skip(rnd.Next(0,_emojies.Count)).First();

        CreateCaptchToDb(idTelegram: idTelegram, content: emoji.Value);

        SendCaptcha(idChat: idChat,
            message: BuildMessesWithCaptcha($"Выберите отличающийся эмодзи"),
            keyboardButtons: GenerateKeyboardEmoji(idChat, emoji.Value, emoji.Key));
    }

    private void CreateCaptchToDb(long idTelegram, string content)
    {
        _captcha = new Captcha
        {
            DateTimeExpired = DateTime.Now.AddMinutes(1),
            IdTelegram = idTelegram,
            Content = content,
            AttemptsRemain = 3
        };
        Repository.Captchas.Add(_captcha);
    }

    private void SendCaptcha(long idChat, string message, IReadOnlyCollection<InlineKeyboardButton> keyboardButtons)
    {
        BotClient.SendTextMessageAsync(chatId: idChat,
            text: message,
            replyMarkup: new InlineKeyboardMarkup(keyboardButtons),
            disableNotification: true);
    }

    private string BuildErrorMessage()
    {
        return
            "\u2705 Вам не требуется проходить проверку на бота, мы Вам доверяем";
    }

    private string BuildMessesWithCaptcha(string messageCaptcha)
    {
        return
            $"\u2734\ufe0f Подтвердите проверку, что ваша учетная запись используется действительно живым человеком " +
            $"пройдя простую капчу: \n\n{messageCaptcha}\n\nЭту капчу необходимо решить до: {_captcha.DateTimeExpired} (1 минута) и 3 попытки";
    }

    private IReadOnlyCollection<InlineKeyboardButton> GenerateKeyboardMathematic(long chatId)
    {
        var rnd = new Random();
        var list = new List<InlineKeyboardButton>();
        for (int i = 0; i < 4; i++)
        {
            var value = rnd.Next(-15, 15);
            list.Add(InlineKeyboardButton.WithCallbackData($"{value}",
                $"captcha {_captcha.Id} {value} {chatId}"));
        }

        list.Add(
            InlineKeyboardButton.WithCallbackData($"{_captcha.Content}",
                $"captcha {_captcha.Id} {_captcha.Content} {chatId}"));
        list = list.OrderBy(_ => rnd.Next()).ToList();
        return list;
    }

    private IReadOnlyCollection<InlineKeyboardButton> GenerateKeyboardEmoji(long chatId, string right, string wrong)
    {
        var rnd = new Random();
        var list = new List<InlineKeyboardButton>();
        for (int i = 0; i < 4; i++)
        {
            list.Add(InlineKeyboardButton.WithCallbackData($"{wrong}", $"captcha {_captcha.Id} {wrong} {chatId}"));
        }

        list.Add(InlineKeyboardButton.WithCallbackData($"{right}", $"captcha {_captcha.Id} {right} {chatId}"));
        list = list.OrderBy(_ => rnd.Next()).ToList();
        return list;
    }
}