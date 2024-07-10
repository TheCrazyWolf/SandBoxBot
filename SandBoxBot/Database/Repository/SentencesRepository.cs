using Microsoft.EntityFrameworkCore;
using SandBoxBot.Models;

namespace SandBoxBot.Database.Repository;

public class SentencesRepository(SandBoxContext ef)
{
    public async Task<bool> IsContainsSentence(string word)
        => await ef.Sentences.AnyAsync(x => x.Value == word.ToLower());
    
    public async Task Add(Sentence sentence)
    {
        if (await IsContainsSentence(sentence.Value.ToLower()))
            return;

        sentence.Value = sentence.Value.ToLower();
        
        await ef.AddRangeAsync(sentence);
        await ef.SaveChangesAsync();
    }
}