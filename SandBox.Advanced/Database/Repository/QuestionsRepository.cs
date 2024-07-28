using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using SandBox.Models.Telegram;

namespace SandBox.Advanced.Database.Repository;

public class QuestionsRepository(SandBoxContext ef)
{
    public async Task<Question> NewQuestionAsync(Question question)
    {
        await ef.AddAsync(question);
        await ef.SaveChangesAsync();
        return question;
    }

    public async Task<Question?> GetByIdAsync(long question)
    {
        return await ef.Questions.FirstOrDefaultAsync(x => x.Id == question);
    }

    /*public Task<IList<Question>> GetByContentQuestion(string searchText)
    {
        var questions = ef.Questions.ToList();

        var results = questions
            .Select(q => new
            {
                Question = q,
                QuestionScore = Fuzz.Ratio(q.Quest, searchText),
                AnswerScore = Fuzz.Ratio(q.Answer, searchText)
            })
            .Select(x => new
            {
                x.Question,
                CombinedScore = Math.Max(x.QuestionScore, x.AnswerScore)
            })
            .Where(x => x.CombinedScore >= 10) // порог схожести, можно настроить
            .OrderByDescending(x => x.CombinedScore)
            .Select(x => x.Question)
            .Take(5)
            .ToList();

        return Task.FromResult<IList<Question>>(results);
    }*/

    public async Task<IList<Question>> GetByContentQuestionAsync(string question)
    {
        var questions = await ef.Questions.ToListAsync();

        var results = questions
            .Select(q => new
            {
                Question = q,
                Score = Fuzz.Ratio(q.Quest, question)
            })
            .Where(x => x.Score >= 15) // порог схожести, можно настроить
            .OrderByDescending(x => x.Score)
            .Select(x => x.Question)
            .Take(5)
            .ToList();

        return results;
    }

    public async Task<Question> UpdateAsync(Question question)
    {
        ef.Update(question);
        await ef.SaveChangesAsync();
        return question;
    }

    public async Task<bool> RemoveAsync(long idQuestion)
    {
        var item = GetByIdAsync(idQuestion).Result;

        if (item is null)
            return false;

        ef.Remove(item);
        await ef.SaveChangesAsync();

        return true;
    }
}