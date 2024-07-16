using SandBox.Models.Telegram;

namespace SandBox.Advanced.Database.Repository;

public class ChatsRepository(SandBoxContext ef)
{
    public Task<ChatTg> Add(ChatTg chat)
    {
        var dbChat = GetById(chat.IdChat).Result;

        if (dbChat is not null)
            return Task.FromResult(dbChat);
        
        ef.Add(chat);
        ef.SaveChanges();
        return Task.FromResult(chat);
    }

    public Task<ChatTg?> GetById(long idChat)
    {
        return Task.FromResult(ef.Chats.FirstOrDefault(x => x.IdChat == idChat));
    }
    
    public Task<bool> Exists(long idChat)
    {
        return Task.FromResult(ef.Chats.Any(x => x.IdChat == idChat));
    }

    public Task<ChatTg> Update(ChatTg chat)
    {
        ef.Update(chat);
        ef.SaveChanges();
        return Task.FromResult(chat);
    }

    public Task<bool> Delete(long idChat)
    {
        var item = GetById(idChat).Result;

        if (item is null)
            return Task.FromResult(false);

        ef.Remove(item);
        ef.SaveChanges();
        
        return Task.FromResult(true);
    }
}