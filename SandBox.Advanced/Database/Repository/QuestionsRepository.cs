using FuzzySharp;
using SandBox.Models.Telegram;

namespace SandBox.Advanced.Database.Repository;

public class QuestionsRepository(SandBoxContext ef)
{
    public Task<Question> Add(Question question)
    {
        ef.Add(question);
        ef.SaveChanges();
        return Task.FromResult(question);
    }

    public Task<Question?> GetById(long question)
    {
        return Task.FromResult(ef.Questions.FirstOrDefault(x => x.Id == question));
    }
    
    public Task<IList<Question>> GetByContentQuestion(string question)
    {
        var questions = ef.Questions.ToList();

        var results = questions
            .Select(q => new
            {
                Question = q,
                Score = Fuzz.Ratio(q.Quest, question)
            })
            .Where(x => x.Score > 40) // порог схожести, можно настроить
            .OrderByDescending(x => x.Score)
            .Select(x => x.Question)
            .Take(5)
            .ToList();

        return Task.FromResult<IList<Question>>(results);
    }

    public Task<Question> Update(Question question)
    {
        ef.Update(question);
        ef.SaveChanges();
        return Task.FromResult(question);
    }

    public Task<bool> Delete(long idQuestion)
    {
        var item = GetById(idQuestion).Result;

        if (item is null)
            return Task.FromResult(false);

        ef.Remove(item);
        ef.SaveChanges();
        
        return Task.FromResult(true);
    }
}