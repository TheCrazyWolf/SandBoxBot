using SandBox.Models.Common;

namespace SandBox.Advanced.Database.Repository;

public class EventRepository(SandBoxContext ef)
{
    public Task<int> GetCountEventsFromIdAccount(long idTelegram, long chatId,
        DateTime dateTimeStart, DateTime dateTimeEnd)
    {
        return Task.FromResult<int>(ef.Events
            .Count(x=> x.IdTelegram == idTelegram 
                       && x.ChatId == chatId
                       && x.DateTime >= dateTimeStart 
                       && x.DateTime<= dateTimeEnd));
    }
    
    public Task<Event?> GetById(long idEvent)
    {
        return Task.FromResult(ef.Events
            .FirstOrDefault(x=> x.Id == idEvent));
    }
}