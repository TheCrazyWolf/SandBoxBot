using Microsoft.EntityFrameworkCore;
using SandBoxBot.Models;

namespace SandBoxBot.Database.Repository;

public class SentencesRepository(SandBoxContext ef)
{
    public async Task<bool> IsContainsSentence(string sentence)
        => await ef.Incidents.AnyAsync(x => x.Value == sentence.ToLower());
    public async Task<Incident?> GetContainsSentence(string sentence)
        => await ef.Incidents.FirstOrDefaultAsync(x=> x.Value == sentence.ToLower());
    
    public async Task<Incident> Add(Incident inc)
    {
        var incident = await GetContainsSentence(inc.Value.ToLower());
        if (incident is not null)
            return incident;

        inc.Value = inc.Value.ToLower();

        await ef.AddRangeAsync(inc);
        await ef.SaveChangesAsync();

        return inc;
    }

    public async Task<Incident?> Get(long id)
    {
        return await ef.Incidents.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Update(Incident inc)
    {
        var incident = await ef.Incidents.FirstOrDefaultAsync(x => x.Id == inc.Id);

        if (incident is null)
            return;

        incident.Value = inc.Value;
        incident.IsSpam = inc.IsSpam;

        ef.Update(incident);
        await ef.SaveChangesAsync();
    }

    public async Task<ICollection<Incident>> GetAll(bool isSpam = true)
    {
        return await ef.Incidents.Where(x => x.IsSpam == isSpam).ToListAsync();
    }
}