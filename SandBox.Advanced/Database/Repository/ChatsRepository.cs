using Microsoft.EntityFrameworkCore;
using SandBox.Models.Chats;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Database.Repository;

public class ChatsRepository(SandBoxContext ef)
{
    public async Task<ChatProps> AddAsync(ChatProps chat)
    {
        await ef.AddAsync(chat);
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

    public async Task<ChatProps> UpdateAsync(ChatProps chat)
    {
        ef.Update(chat);
        await ef.SaveChangesAsync();
        return chat;
    }

    public async Task<ChatProps> NewChatOrUpdateAsync(Chat chat)
    {
        var db = await GetByIdAsync(chat.Id);

        if (db is null)
        {
            db = new()
            {
                Title = chat.Title,
                Username = chat.Username,
                FirstName = chat.FirstName,
                LastName = chat.LastName,
                Type = chat.Type,
                IdChat = chat.Id,
            };

            await AddAsync(db);
            return db;
        }

        db.Title = chat.Title;
        db.Username = chat.Username;
        db.FirstName = chat.FirstName;
        db.LastName = chat.LastName;
        db.Type = chat.Type;
        await UpdateAsync(db);
        return db;
    }

    public async Task<bool> RemoveAsync(long idChat)
    {
        var item = await GetByIdAsync(idChat);

        if (item is null)
            return false;

        ef.Remove(item);
        await ef.SaveChangesAsync();
        return true;
    }
}