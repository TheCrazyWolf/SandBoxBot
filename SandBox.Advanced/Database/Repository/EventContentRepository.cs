using SandBox.Models.Events;

namespace SandBox.Advanced.Database.Repository;

public class EventContentRepository(SandBoxContext ef)
{
    public Task<EventContent> Add(EventContent @event)
    {
        ef.Add(@event);
        ef.SaveChanges();
        return Task.FromResult(@event);
    }

    public Task<EventContent?> GetById(long idEvent)
    {
        return Task.FromResult(ef.EventsContent.FirstOrDefault(x => x.Id == idEvent));
    }
    
    public Task<bool> Exists(long idEvent)
    {
        return Task.FromResult(ef.EventsContent.Any(x => x.Id == idEvent));
    }

    public Task<EventContent> Update(EventContent blackWord)
    {
        ef.Update(blackWord);
        ef.SaveChanges();
        return Task.FromResult(blackWord);
    }

    public Task<bool> Delete(long idEvent)
    {
        var item = GetById(idEvent).Result;

        if (item is null)
            return Task.FromResult(false);

        ef.Remove(item);
        ef.SaveChanges();
        
        return Task.FromResult(true);
    }
}