using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using SandBox.Models.Telegram;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectQuestion : SandBoxHelpers, IExecutable<bool>
{
    private readonly long _idTelegramTrainer = 1946031755;
    
    public Task<bool> Execute()
    {
        if (Update.Message?.ReplyToMessage is null && Update.Message?.From?.Id != _idTelegramTrainer && Update.Message?.Text is not null)
            return Task.FromResult(false);

        SaveAnswerForQuestion();
       
        return Task.FromResult(true);
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8601 // Possible null reference assignment.
    private Task SaveAnswerForQuestion()
    {
        var quest = new Question
        {
            Quest = Update.Message?.ReplyToMessage.Text,
            Answer = Update.Message?.Text
        };

        Repository.Questions.Add(quest);
        return Task.CompletedTask;
    }
    
    
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    
}