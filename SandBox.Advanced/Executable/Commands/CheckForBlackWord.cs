using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Services.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class CheckForBlackWord(
    ITelegramBotClient botClient,
    Update update,
    SandBoxRepository repository) : IExecutable
{

    private bool _isBlackKeyWord;
    private string _blackWords = string.Empty;

    public Task Execute()
    {
        if (update.Message?.From is null)
            return Task.CompletedTask;

        Proccess();
        SendMessage(BuildSuccessMessage());
        
        return Task.CompletedTask;
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
        var resultMsgForBlackList = _isBlackKeyWord ? "Да" : "Нет";
        return
            $"\u2705 Команда выполнена" +
            $"\n\nАлгоритм ключевых слов: {resultMsgForBlackList}" +
            $": {_blackWords}\n\n";
    }
    
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}