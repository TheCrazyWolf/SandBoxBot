using Microsoft.EntityFrameworkCore;
using SandBox.Models.Common;
using SandBox.Models.Events;

namespace SandBox.Advanced.Database.Repository;

public class EventContentRepository(SandBoxContext ef)
{
    public Task<EventContent> Add(EventContent @event)
    {
        /*
        var foundContent = GetByContent(@event.Content).Result;
        if (foundContent is not null)
            return Task.FromResult(foundContent);
            */
        
        ef.Add(@event);
        ef.SaveChanges();
        return Task.FromResult(@event);
    }

    public Task<EventContent?> GetById(long idEvent)
    {
        return Task.FromResult(ef.EventsContent.FirstOrDefault(x => x.Id == idEvent));
    }
    
    public Task<EventContent?> GetByContent(string? content)
    {
        return Task.FromResult(ef.EventsContent.FirstOrDefault(x => x.Content == content));
    }
    
    public Task<EventContent?> GetByContent(string? content, long userId, long chaId, long messageId)
    {
        return Task.FromResult(ef.EventsContent.FirstOrDefault(x => x.Content == content && x.IdTelegram == userId && x.ChatId == chaId && x.MessageId == messageId));
    }
    
    public Task<bool> Exists(long idEvent)
    {
        return Task.FromResult(ef.EventsContent.Any(x => x.Id == idEvent));
    }

    public Task<EventContent> UpdateAsync(EventContent blackWord)
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

    public void UpdateNoSpam(EventContent @event)
    {
        @event.IsSpam = false;
        UpdateAsync(@event);
    }

    public async Task<int> CountMessageFromUser(long userId, bool isSpam)
    {
        return await ef.EventsContent.CountAsync(x => x.IdTelegram == userId && x.IsSpam == isSpam);
    }
}