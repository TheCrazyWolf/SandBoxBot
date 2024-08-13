using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using SandBox.Models.Telegram;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Analyzers.Misc;

public class TrainerSgk(SandBoxRepository repository, 
    long idTrainer, long idChat) : IAnalyzer
{
    public void Execute(Message message)
    {
        if(message.ReplyToMessage?.Text is null) return;
        
        if(message.Chat.Id != idChat) return;
        
        if(message.Text is null) return;
        
        if(message.From?.Id != idTrainer) return;
        
        if (message.Text != null)
            SaveAnswerForQuestion(question: message.ReplyToMessage.Text,
                answer: message.Text);
    }
    
    
    private async void SaveAnswerForQuestion(string question, string answer)
    {
        var quest = new Question
        {
            Quest = question.GetMessageForFaq(),
            Answer = answer.GetMessageForFaq()
        };

        await repository.Questions.NewQuestionAsync(quest);
    }
    
    
}