using Microsoft.EntityFrameworkCore;

namespace SandBox.Advanced.Database.Repository;

public class EventRepository(SandBoxContext ef)
{
    public async Task<int> GetCountEventsFromIdAccountAsync(long idTelegram, long chatId,
        DateTime dateTimeStart, DateTime dateTimeEnd)
    {
        return await ef.Events
            .CountAsync(x=> x.IdTelegram == idTelegram 
                       && x.ChatId == chatId
                       && x.DateTime >= dateTimeStart 
                       && x.DateTime<= dateTimeEnd);
    }
}