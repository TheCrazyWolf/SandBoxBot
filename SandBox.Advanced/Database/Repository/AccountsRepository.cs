using SandBox.Models.Telegram;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Database.Repository;

public class AccountsRepository(SandBoxContext ef)
{
    public Task<Account> Add(Account account)
    {
        var dbAccount = GetById(account.IdTelegram).Result;

        if (dbAccount is not null)
            return Task.FromResult(dbAccount);
        
        ef.Add(account);
        ef.SaveChanges();
        return Task.FromResult(account);
    }

    public Task<Account?> GetById(long idTelegram)
    {
        return Task.FromResult(ef.Accounts.FirstOrDefault(x => x.IdTelegram == idTelegram));
    }

    public Task<List<Account>> GetManagers()
    {
        return Task.FromResult(ef.Accounts.Where(x=> x.IsManagerThisBot == true).ToList());
    }
    
    public Task<bool> Exists(long idTelegram)
    {
        return Task.FromResult(ef.Accounts.Any(x => x.IdTelegram == idTelegram));
    }

    public Task<Account> Update(Account blackWord)
    {
        ef.Update(blackWord);
        ef.SaveChanges();
        return Task.FromResult(blackWord);
    }

    public Task<bool> Delete(long idTelegram)
    {
        var item = GetById(idTelegram).Result;

        if (item is null)
            return Task.FromResult(false);

        ef.Remove(item);
        ef.SaveChanges();
        
        return Task.FromResult(true);
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
}