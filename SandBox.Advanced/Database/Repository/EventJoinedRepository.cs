using Microsoft.EntityFrameworkCore;
using SandBox.Models.Events;

namespace SandBox.Advanced.Database.Repository;

public class EventsJoinedRepository(SandBoxContext ef)
{
    public async Task<EventJoined> NewJoinAsync(EventJoined @event)
    {
        await ef.AddAsync(@event);
        await ef.SaveChangesAsync();
        return @event;
    }

    public async Task<EventJoined?> GetByIdAsync(long idEvent)
    {
        return await ef.EventsJoined.FirstOrDefaultAsync(x => x.Id == idEvent);
    }

    public async Task<bool> ExistsAsync(long idEvent)
    {
        return await ef.EventsJoined.AnyAsync(x => x.Id == idEvent);
    }

    public async Task<EventJoined> UpdateAsync(EventJoined eventJoin)
    {
        ef.Update(eventJoin);
        await ef.SaveChangesAsync();
        return eventJoin;
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

    public async Task<int> GetCountJoinsFromChatAsync(long chatId, DateTime start, DateTime end)
    {
        return await ef.EventsJoined
            .CountAsync(x => x.ChatId == chatId
                             && x.DateTime >= start
                             && x.DateTime <= end);
    }
}