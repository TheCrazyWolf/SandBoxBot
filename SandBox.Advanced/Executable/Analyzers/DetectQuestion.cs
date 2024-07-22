using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using SandBox.Models.Telegram;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectQuestion(SandBoxRepository repository, 
    long idTrainer, long idChat) : IAnalyzer
{
    public bool Execute(Message message)
    {
        if(message.ReplyToMessage?.Text is null)
            return false;
        
        if(message.Chat.Id != idChat)
            return false;
        
        if(message.Text is null)
            return false;
        
        if(message.From?.Id != idTrainer)
            return false;
        
        if (message.Text != null)
            SaveAnswerForQuestion(question: message.ReplyToMessage.Text,
                answer: message.Text);

        return true;
    }
    
    
    private void SaveAnswerForQuestion(string question, string answer)
    {
        var quest = new Question
        {
            Quest = question.GetMessageForFaq(),
            Answer = answer.GetMessageForFaq()
        };

        repository.Questions.Add(quest);
    }
    
    
}