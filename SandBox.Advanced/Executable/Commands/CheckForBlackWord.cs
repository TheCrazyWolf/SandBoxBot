using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Services.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class CheckForBlackWord(
    ITelegramBotClient botClient,
    Update update,
    SandBoxRepository repository) : IExecutable<bool>
{

    private bool _isBlackKeyWord;
    private bool _isSpamFromMl;
    private float _score;
    private string _blackWords = string.Empty;

    public Task<bool> Execute()
    {
        if (update.Message?.From is null)
            return Task.FromResult(false);

        Proccess();
        _isSpamFromMl = IsSpamPredict(update.Message?.Text);
        
        SendMessage(BuildSuccessMessage());
        return Task.FromResult(true);
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.

    private Task Proccess()
    {
        // проверка, чтобы не добавлялись команды - SKIP 1
        var words = TextTreatment.GetArrayWordsTreatmentMessage(update.Message.Text).Skip(1);

        foreach (var word in words)
        {
            var result = repository.BlackWords.Exists(word).Result;

            if (!result) continue;
            _isBlackKeyWord = true;
            _blackWords += $"{word}, ";

        }

        return Task.CompletedTask;
    }

    private Task SendMessage(string message)
    {
        botClient.SendTextMessageAsync(chatId:update.Message.Chat.Id,
            text: message,
            disableNotification: true);
        return Task.CompletedTask;
    }

    private string BuildSuccessMessage()
    {
        var resultMsgForBlackList = _isBlackKeyWord ? "\ud83d\udeab" : "\u2705";
        var resultFromMl = _isSpamFromMl ? "\ud83d\udeab" : "\u2705";
        var resultMessage = _isSpamFromMl || _isBlackKeyWord
            ? "\u26a0\ufe0f Похоже, что это является спамом и подлежит блокировке"
            : "\u2705 Похоже, что это обычное сообщение";
        return
            $"\u2705 Команда выполнена" +
            $"\n\nРезультаты распознавания текста: " +
            $"\n\n\u26a1\ufe0f Алгоритм ключевых слов: {resultMsgForBlackList}" +
            $":\n{_blackWords}\n\n" +
            $"\u26a1\ufe0f Модель машинного обучения: {resultFromMl}\nВероятность {_score}%" +
            $"\n\n{resultMessage}";
    }
    
    private bool IsSpamPredict(string? message)
    {
        var result = MlPredictor.IsSpamPredict(message);
        _score = result.Item2;
        return result.Item1;
    }
    
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}