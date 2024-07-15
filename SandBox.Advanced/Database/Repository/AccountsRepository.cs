using SandBox.Models.Telegram;

namespace SandBox.Advanced.Database.Repository;

public class AccountsRepository(SandBoxContext ef)
{
    public Task<Account> Add(Account account)
    {
        
        ef.Add(account);
        ef.SaveChanges();
        return Task.FromResult(account);
    }

    public Task<Account?> GetById(long idTelegram)
    {
        return Task.FromResult(ef.Accounts.FirstOrDefault(x => x.AccountIdTelegram == idTelegram));
    }
    
    public Task<bool> Exists(long idTelegram)
    {
        return Task.FromResult(ef.Accounts.Any(x => x.AccountIdTelegram == idTelegram));
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
}