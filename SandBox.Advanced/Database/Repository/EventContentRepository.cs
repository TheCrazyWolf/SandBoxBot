using Microsoft.EntityFrameworkCore;
using SandBox.Models.Events;

namespace SandBox.Advanced.Database.Repository;

public class EventContentRepository(SandBoxContext ef)
{
    public async Task<EventContent> NewContentAsync(EventContent @event)
    {
        await ef.AddAsync(@event);
        await ef.SaveChangesAsync();
        return @event;
    }

    public async Task<EventContent?> GetByIdAsync(long idEvent)
    {
        return await ef.EventsContent.FirstOrDefaultAsync(x => x.Id == idEvent);
    }

    public async Task<EventContent?> GetByContentAsync(string? content)
    {
        return await ef.EventsContent.FirstOrDefaultAsync(x => x.Content == content);
    }

    public async Task<EventContent?> GetByContentAsync(string? content, long userId, long chaId, long messageId)
    {
        return await (ef.EventsContent.FirstOrDefaultAsync(x =>
            x.Content == content && x.IdTelegram == userId && x.ChatId == chaId && x.MessageId == messageId));
    }

    public async Task<bool> ExistsAsync(long idEvent)
    {
        return await ef.EventsContent.AnyAsync(x => x.Id == idEvent);
    }

    public async Task<EventContent> UpdateAsync(EventContent blackWord)
    {
        ef.Update(blackWord);
        await ef.SaveChangesAsync();
        return blackWord;
    }

    public async Task<bool> RemoveAsync(long idEvent)
    {
        var item = GetByIdAsync(idEvent).Result;

        if (item is null)
            return false;

        ef.Remove(item);
        await ef.SaveChangesAsync();

        return true;
    }

    public async void UpdateNoSpamAsync(EventContent @event)
    {
        @event.IsSpam = false;
        await UpdateAsync(@event);
    }
    
    public async void UpdateRestored(EventContent @event)
    {
        @event.IsRestored = true;
        await UpdateAsync(@event);
    }

    public async Task<int> CountMessageFromUserAsync(long userId, bool isSpam)
    {
        return await ef.EventsContent.CountAsync(x => x.IdTelegram == userId && x.IsSpam == isSpam);
    }
}