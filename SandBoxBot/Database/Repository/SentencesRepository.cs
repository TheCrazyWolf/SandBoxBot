using Microsoft.EntityFrameworkCore;
using SandBoxBot.Models;

namespace SandBoxBot.Database.Repository;

public class SentencesRepository(SandBoxContext ef)
{
    public async Task<bool> IsContainsSentence(string sentence)
        => await ef.Sentences.AnyAsync(x => x.Value == sentence.ToLower());
    public async Task<Sentence?> GetContainsSentence(string sentence)
        => await ef.Sentences.FirstOrDefaultAsync(x=> x.Value == sentence.ToLower());
    

    public async Task<Sentence> Add(Sentence sentence)
    {
        var incident = await GetContainsSentence(sentence.Value.ToLower());
        if (incident is not null)
            return incident;

        sentence.Value = sentence.Value.ToLower();

        await ef.AddRangeAsync(sentence);
        await ef.SaveChangesAsync();

        return sentence;
    }

    public async Task<Sentence?> Get(long id)
    {
        return await ef.Sentences.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Update(Sentence sentence)
    {
        var incident = await ef.Sentences.FirstOrDefaultAsync(x => x.Id == sentence.Id);

        if (incident is null)
            return;

        incident.Value = sentence.Value;
        incident.IsSpam = sentence.IsSpam;

        ef.Update(incident);
        await ef.SaveChangesAsync();
    }
}