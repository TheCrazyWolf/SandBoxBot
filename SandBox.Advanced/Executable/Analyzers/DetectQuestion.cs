using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Interfaces;
using SandBox.Models.Telegram;
namespace SandBox.Advanced.Executable.Analyzers;

public class DetectQuestion : SandBoxHelpers, IExecutable<bool>
{
    //my id 208049718
    //1946031755
    private readonly long _idTelegramTrainer = 1946031755;

    public Task<bool> Execute()
    {
        if(Update.Message?.ReplyToMessage?.Text is null)
            return Task.FromResult(false);
        
        if(Update.Message?.Text is null)
            return Task.FromResult(false);
        
        if(Update.Message?.From?.Id != _idTelegramTrainer)
            return Task.FromResult(false);
        
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

        while (newMessage.StartsWith("!") || newMessage.StartsWith(".") || newMessage.StartsWith(",") || newMessage.StartsWith(" ") || newMessage.StartsWith("\n"))
        {
            newMessage = newMessage.Substring(1);
        }

        return newMessage;
    }
    
}