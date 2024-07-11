using Microsoft.EntityFrameworkCore;
using SandBoxBot.Models;

namespace SandBoxBot.Database.Repository;

public class BlackWordRepository(SandBoxContext ef)
{
    
    public async Task<bool> IsContainsWord(string word)
        => await ef.BlackWords.AnyAsync(x => x.Word == word.ToLower());


    public async Task<ICollection<BlackWord>> GetAll()
    {
        return await ef.BlackWords.ToListAsync();
    }

    public async Task Add(string word)
    {
        if (await IsContainsWord(word.ToLower()))
            return;

        var blackWord = new BlackWord
        {
            Word = word.ToLower()
        };

        await ef.AddRangeAsync(blackWord);
        await ef.SaveChangesAsync();
    }

    public async Task Delete(string word)
    {
        var foundWord = await SandBoxContext.Instance.BlackWords
            .FirstOrDefaultAsync(x => x.Word == word.ToLower());
        
        if(foundWord is null)
            return;

        ef.Remove(foundWord);
        await ef.SaveChangesAsync();
    }
}