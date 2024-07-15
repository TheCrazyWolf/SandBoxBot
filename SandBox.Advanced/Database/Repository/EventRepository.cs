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
}