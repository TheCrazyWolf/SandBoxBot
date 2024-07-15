using SandBox.Models.Blackbox;

namespace SandBox.Advanced.Database.Repository;

public class BlackWordRepository(SandBoxContext ef)
{
    public Task<BlackWord> Add(BlackWord blackWord)
    {
        var dBlackWord = GetByWord(blackWord.Content.ToLower()).Result;
        
        if (dBlackWord is not null)
            return Task.FromResult(dBlackWord);

        blackWord.Content = blackWord.Content.ToLower();
        ef.Add(blackWord);
        ef.SaveChanges();
        return Task.FromResult(blackWord);
    }

    public Task<BlackWord?> GetByWord(string blackWord)
    {
        return Task.FromResult(ef.BlackWords.FirstOrDefault(x => x.Content == blackWord.ToLower()));
    }

    public Task<bool> Exists(string blackWord)
    {
        return Task.FromResult(ef.BlackWords.Any(x => x.Content == blackWord.ToLower()));
    }

    public Task<BlackWord> Update(BlackWord blackWord)
    {
        blackWord.Content = blackWord.Content.ToLower();
        ef.Update(blackWord);
        ef.SaveChanges();
        return Task.FromResult(blackWord);
    }

    public Task<bool> Delete(string blackWord)
    {
        var item = GetByWord(blackWord).Result;

        if (item is null)
            return Task.FromResult(false);

        ef.Remove(item);
        ef.SaveChanges();

        return Task.FromResult(true);
    }
}