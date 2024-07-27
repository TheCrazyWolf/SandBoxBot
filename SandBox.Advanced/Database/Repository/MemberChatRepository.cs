using System.Collections;
using Microsoft.EntityFrameworkCore;
using SandBox.Models.Telegram;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Database.Repository;

public class MemberChatRepository(SandBoxContext ef)
{
    public async Task<MemberInChat> NewMemberOrUpdateInChatAsync(long idChat, User user)
    {
        var member = await GetByIdAsync(idChat: idChat, idTelegram: user.Id) ?? new MemberInChat()
        {
            IdChat = idChat,
            IdTelegram = user.Id,
            LastActivity = DateTime.Now,
            DateTimeJoined = DateTime.Now,
            CountMessage = 0,
            IsRestricted = false
        };

        member.LastActivity = DateTime.Now;
        ef.Update(member);
        await ef.SaveChangesAsync();
        return member;
    }

    public async Task<MemberInChat?> GetByIdAsync(long idChat, long idTelegram)
    {
        return await ef.MembersInChats.FirstOrDefaultAsync(x => x.IdChat == idChat && x.IdTelegram == idTelegram);
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await ef.MembersInChats.AnyAsync(x => x.Id == id);
    }

    public async Task<MemberInChat> UpdateAsync(MemberInChat member)
    {
        ef.Update(member);
        await ef.SaveChangesAsync();
        return member;
    }
    
    public async Task<MemberInChat> UpdateAsync(long idChat, User user)
    {
        var member = await GetByIdAsync(idChat: idChat, idTelegram: 
            user.Id) ?? new MemberInChat()
        {
            IdChat = idChat,
            CountMessage = 0,
            IsRestricted = false,
            LastActivity = DateTime.Now,
            DateTimeJoined = DateTime.Now,
        };

        member.LastActivity = DateTime.Now;
        ef.Update(member);
        await ef.SaveChangesAsync();
        return member;
    }

    public async Task<bool> RemoveAsync(long idChat, long idTelegram)
    {
        var item = await GetByIdAsync(idChat: idChat, idTelegram: idTelegram);

        if (item is null)
            return false;

        ef.Remove(item);
        await ef.SaveChangesAsync();
        return true;
    }

    public async void UpdateAprrovedAsync(MemberInChat member)
    {
        member.IsApproved = true;
        await UpdateAsync(member);
    }

    public async Task UpdateIsAdmin(MemberInChat memberInChat, bool result)
    {
        memberInChat.IsAdmin = true;
        memberInChat.IsApproved = false;
        memberInChat.IsRestricted = false;
        await UpdateAsync(memberInChat);
    }

    public async Task<IList<MemberInChat>> GetAdminsFromChat(long chatId)
    {
        return await ef.MembersInChats.Where(x => x.IdChat == chatId)
            .Where(x => x.IsAdmin).ToListAsync();
    }
}