using Microsoft.EntityFrameworkCore;
using SandBox.Models.Members;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Database.Repository;

public class AccountsRepository(SandBoxContext ef)
{
    public async Task<Account> AddNewAsync(Account account)
    {
        await ef.AddAsync(account);
        await ef.SaveChangesAsync();
        return account;
    }

    public async Task<Account> NewUserOrUpdateAsync(User user)
    {
        var dbAccount = await GetByIdAsync(user.Id);

        if (dbAccount is null)
        {
            dbAccount = new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.Username,
                IdTelegram = user.Id,
            };
            await AddNewAsync(dbAccount);
            return dbAccount;
        }

        dbAccount.FirstName = user.FirstName;
        dbAccount.LastName = user.LastName;
        dbAccount.UserName = user.Username;
        dbAccount.IdTelegram = user.Id;
        ef.Update(dbAccount);
        await ef.SaveChangesAsync();
        return dbAccount;
    }

    public async Task<Account?> GetByIdAsync(long idTelegram)
    {
        return await ef.Accounts.FirstOrDefaultAsync(x => x.IdTelegram == idTelegram);
    }

    public async Task<List<Account>> GetManagersAsync()
    {
        return await ef.Accounts.Where(x => x.IsManagerThisBot == true).ToListAsync();
    }

    public async Task<bool> ExistsAsync(long idTelegram)
    {
        return await ef.Accounts.AnyAsync(x => x.IdTelegram == idTelegram);
    }

    public async Task<Account> UpdateAsync(Account account)
    {
        ef.Update(account);
        await ef.SaveChangesAsync();
        return account;
    }

    public async Task<bool> RemoveAsync(long idTelegram)
    {
        var item = GetByIdAsync(idTelegram).Result;

        if (item is null)
            return false;

        ef.Remove(item);
        await ef.SaveChangesAsync();

        return true;
    }

    public async void UpdateApprovedAsync(Account account)
    {
        account.IsGlobalApproved = true;
        account.IsGlobalRestricted = false;
        await UpdateAsync(account);
    }

    public async void UpdateAdminAsync(Account account)
    {
        account.IsGlobalApproved = true;
        account.IsGlobalRestricted = false;
        account.IsManagerThisBot = true;
        await UpdateAsync(account);
    }

    public async void UpdateDetailsAsync(Account account, User user)
    {
        account.FirstName = user.FirstName;
        account.LastName = user.LastName;
        account.UserName = user.Username;
        await UpdateAsync(account);
    }

    public async void UpdateRestrictedAsync(Account account)
    {
        account.IsGlobalApproved = false;
        account.IsGlobalRestricted = true;
        await UpdateAsync(account);
    }
}