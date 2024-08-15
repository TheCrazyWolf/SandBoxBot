using System.Collections;
using Microsoft.EntityFrameworkCore;
using SandBox.Models.Members;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Database.Repository;

public class MemberChatRepository(SandBoxContext ef)
{
    public async Task<MemberChat> AddAsync(MemberChat memberChat)
    {
        await ef.AddAsync(memberChat);
        await ef.SaveChangesAsync();
        return memberChat;
    }

    public async Task<MemberChat> NewMemberOrUpdateInChatAsync(long idChat, User user)
    {
        var member = await GetByIdAsync(idChat: idChat, idTelegram: user.Id);

        if (member is null)
        {
            member = new MemberChat()
            {
                IdChat = idChat,
                IdTelegram = user.Id,
                LastActivity = DateTime.Now,
                DateTimeJoined = DateTime.Now,
                IsRestricted = false
            };
            await AddAsync(member);
            return member;
        }

        member.LastActivity = DateTime.Now;
        ef.Update(member);
        await ef.SaveChangesAsync();
        return member;
    }

    public async Task<MemberChat?> GetByIdAsync(long idChat, long idTelegram)
    {
        return await ef.MembersInChats.FirstOrDefaultAsync(x => x.IdChat == idChat && x.IdTelegram == idTelegram);
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await ef.MembersInChats.AnyAsync(x => x.Id == id);
    }

    public async Task<MemberChat> UpdateAsync(MemberChat member)
    {
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

    public async void UpdateAprrovedAsync(MemberChat member)
    {
        member.IsApproved = true;
        member.IsRestricted = false;
        await UpdateAsync(member);
    }

    public async Task UpdateIsAdmin(MemberChat memberChat, bool result)
    {
        memberChat.IsAdmin = result;
        if (result)
        {
            memberChat.IsApproved = result;
            memberChat.IsRestricted = false;
        }

        await UpdateAsync(memberChat);
    }

    public async Task<IList<MemberChat>> GetAdminsFromChat(long chatId)
    {
        return await ef.MembersInChats.Where(x => x.IdChat == chatId)
            .Where(x => x.IsAdmin).ToListAsync();
    }

    public async void UpdateRestrictedAsync(MemberChat memberChat)
    {
        memberChat.IsApproved = false;
        memberChat.IsRestricted = true;
        await UpdateAsync(memberChat);
    }
}