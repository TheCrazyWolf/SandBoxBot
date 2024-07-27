using Microsoft.EntityFrameworkCore;
using SandBox.Models.Telegram;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Database.Repository;

public class AccountsRepository(SandBoxContext ef)
{
    public async Task<Account> NewUserOrUpdateAsync(User user)
    {
        var dbAccount = await GetByIdAsync(user.Id) ?? new Account();
        dbAccount.FirstName = user.FirstName;
        dbAccount.LastName = user.LastName;
        dbAccount.UserName = user.Username;
        ef.Update(dbAccount);
        await ef.SaveChangesAsync();
        return dbAccount;
    }

    public async Task<Account?> GetByIdAsync(long idTelegram)
    {
        return await ef.Accounts.FirstOrDefaultAsync(x => x.IdTelegram == idTelegram);
    }

    public async Task<List<Account>> GetManagers()
    {
        return await ef.Accounts.Where(x=> x.IsManagerThisBot == true).ToListAsync();
    }
    
    public async Task<bool> ExistsAsync(long idTelegram)
    {
        return await ef.Accounts.AnyAsync(x => x.IdTelegram == idTelegram);
    }

    public async Task<Account> Update(Account account)
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

    public void UpdateApproved(Account account)
    {
        account.IsAprroved = true;
        account.IsNeedToVerifyByCaptcha = false;
        account.IsSpamer = false;
        Update(account);
    }

    public void UpdateAdmin(Account account)
    {
        account.IsAprroved = true;
        account.IsNeedToVerifyByCaptcha = false;
        account.IsSpamer = false;
        account.IsManagerThisBot = true;
        Update(account);
    }

    public void UpdateDetails(Account account, User user)
    {
        account.FirstName = user.FirstName;
        account.LastName = user.LastName;
        account.UserName = user.Username;
        account.LastActivity = DateTime.Now;
        Update(account);
    }

    public void UpdateToSpamer(Account account)
    {
        account.IsSpamer = true;
        account.IsNeedToVerifyByCaptcha = true;
        Update(account);
    }
}