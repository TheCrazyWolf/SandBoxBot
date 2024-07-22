using SandBox.Models.Events;
namespace SandBox.Advanced.Database.Repository;

public class EventsJoinedRepository(SandBoxContext ef)
{
    public Task<EventJoined> Add(EventJoined @event)
    {
        ef.Add(@event);
        ef.SaveChanges();
        return Task.FromResult(@event);
    }

    public Task<EventJoined?> GetById(long idEvent)
    {
        return Task.FromResult(ef.EventsJoined.FirstOrDefault(x => x.Id == idEvent));
    }

    public Task<bool> Exists(long idEvent)
    {
        return Task.FromResult(ef.EventsJoined.Any(x => x.Id == idEvent));
    }

    public Task<EventJoined> Update(EventJoined blackWord)
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

    public int GetCountJoinsFromChat(long chatId, DateTime start, DateTime end)
    {
        return ef.EventsJoined
            .Count(x => x.ChatId == chatId
                        && x.DateTime >= start
                        && x.DateTime <= end);
    }
}