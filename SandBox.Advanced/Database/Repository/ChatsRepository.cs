using Microsoft.EntityFrameworkCore;
using SandBox.Models.Telegram;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Database.Repository;

public class ChatsRepository(SandBoxContext ef)
{
    public async Task<ChatProps> AddAsync(ChatProps chat)
    {
        var dbChat = GetByIdAsync(chat.IdChat).Result;

        if (dbChat is not null)
            return dbChat;

        ef.Add(chat);
        await ef.SaveChangesAsync();
        return chat;
    }

    public async Task<ChatProps?> GetByIdAsync(long idChat)
    {
        return await ef.Chats.FirstOrDefaultAsync(x => x.IdChat == idChat);
    }

    public async Task<bool> ExistsAsync(long idChat)
    {
        return await ef.Chats.AnyAsync(x => x.IdChat == idChat);
    }

    public async Task<ChatProps> Update(ChatProps chat)
    {
        ef.Update(chat);
        await ef.SaveChangesAsync();
        return chat;
    }
    
    public async Task<ChatProps> Update(Chat chat)
    {
        var db = await GetByIdAsync(chat.Id) ?? new ChatProps();

        db.Title = chat.Title;
        db.Username = chat.Username;
        db.FirstName = chat.FirstName;
        db.LastName = chat.LastName;
        db.Type = chat.Type;
        ef.Update(chat);
        await ef.SaveChangesAsync();
        return db;
    }

    public async Task<bool> Delete(long idChat)
    {
        var item = await GetByIdAsync(idChat);

        if (item is null)
            return false;

        ef.Remove(item);
        await ef.SaveChangesAsync();
        return true;
    }
}