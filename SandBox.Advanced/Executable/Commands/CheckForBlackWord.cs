using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Services.Text;
using SandBox.Advanced.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class CheckForBlackWord(SandBoxRepository repository, ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/check";

    public override void Execute(Message message)
    {
        if (message.From is null) return;

        var forBlackWords = CheckForBlackWords(message.Text);
        var isBlackWords = string.IsNullOrEmpty(forBlackWords);
        var isMlNet = message.Text.IsSpamMl();

        var buildMessage = BuildMessage(isBlackWord : isBlackWords,  
            isSpamMl : isMlNet.Item1, 
            score: isMlNet.Item2, blackWords: forBlackWords);
        
        SendMessage(message.Chat.Id, buildMessage);
    }
    
    private string CheckForBlackWords(string? message)
    {
        var words = message?.GetArrayWordsTreatmentMessage(1);
        string blackWords = (from word in words
                let result =
                    repository.BlackWords.Exists(word).Result
                where result
                select word)
            .Aggregate(string.Empty, (current, word) => current + $"{word}, ");

        return blackWords;
    }

    private void SendMessage(long idChat, string message)
    {
        botClient.SendTextMessageAsync(chatId: idChat,
            text: message,
            disableNotification: true);
    }

    private string BuildMessage(bool isBlackWord, bool isSpamMl, float score, string? blackWords)
    {
        var resultMsgForBlackList = isBlackWord ? "\ud83d\udeab" : "\u2705";
        var resultFromMl = isSpamMl ? "\ud83d\udeab" : "\u2705";
        var resultMessage = isSpamMl || isBlackWord
            ? "\u26a0\ufe0f Похоже, что это является спамом и подлежит блокировке"
            : "\u2705 Похоже, что это обычное сообщение";
        return
            $"\u2705 Команда выполнена" +
            $"\n\nРезультаты распознавания текста: " +
            $"\n\n\u26a1\ufe0f Алгоритм ключевых слов: {resultMsgForBlackList}" +
            $":\n{blackWords}\n\n" +
            $"\u26a1\ufe0f Модель машинного обучения: {resultFromMl}\nВероятность {score}%" +
            $"\n\n{resultMessage}";
    }
}