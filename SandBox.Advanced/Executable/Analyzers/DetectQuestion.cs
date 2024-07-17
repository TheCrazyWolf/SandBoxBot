using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using SandBox.Models.Telegram;
namespace SandBox.Advanced.Executable.Analyzers;

public class DetectQuestion : SandBoxHelpers, IExecutable<bool>
{
    private readonly long _idTelegramTrainer = 1946031755;

    public Task<bool> Execute()
    {
        if (Update.Message?.ReplyToMessage is null && Update.Message?.From?.Id != _idTelegramTrainer &&
            Update.Message?.Text is not null)
            return Task.FromResult(false);

        if (Update.Message?.ReplyToMessage?.Text == null) return Task.FromResult(true);


        if (Update.Message.Text != null)
            SaveAnswerForQuestion(question: Update.Message.ReplyToMessage.Text,
                answer: Update.Message.Text);

        return Task.FromResult(true);
    }

    private void SaveAnswerForQuestion(string question, string answer)
    {
        var quest = new Question
        {
            Quest = GetMessagePreparing(question),
            Answer = GetMessagePreparing(answer)
        };

        Repository.Questions.Add(quest);
    }

    private string GetMessagePreparing(string message)
    {
        var newMessage = message.Replace("Здравствуйте", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("доброе утро", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("добрый день", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("добрый вечер", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("доброй ночи", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (newMessage.StartsWith("!") || newMessage.StartsWith(".") || newMessage.StartsWith(",") || newMessage.StartsWith(" "))
        {
            newMessage = newMessage.Substring(1);
        }

        return newMessage;
    }
    
}