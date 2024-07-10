using Microsoft.EntityFrameworkCore;
using SandBoxBot.Models;

namespace SandBoxBot.Database.Repository;

public class BlackWordRepository(SandBoxContext ef)
{
    private SandBoxContext _ef = ef;

    public async Task<bool> IsContainsWord(string word)
        => await _ef.BlackWords.AnyAsync(x => x.Word == word.ToLower());

    public async Task Add(string word)
    {
        if (await IsContainsWord(word.ToLower()))
            return;

        var blackWord = new BlackWord
        {
            Word = word.ToLower()
        };

        await _ef.AddRangeAsync(blackWord);
        await _ef.SaveChangesAsync();
    }

    public async Task Delete(string word)
    {
        var foundWord = await SandBoxContext.Instance.BlackWords
            .FirstOrDefaultAsync(x => x.Word == word.ToLower());
        
        if(foundWord is null)
            return;

        _ef.Remove(foundWord);
        await _ef.SaveChangesAsync();
    }
}