using Microsoft.EntityFrameworkCore;
using SandBoxBot.Models;

namespace SandBoxBot.Database.Repository;

public class AccountRepository(SandBoxContext ef)
{
    public async Task<bool> ExistAccount(long id)
        => await ef.Accounts.AnyAsync(x => x.IdAccountTelegram == id);

    public async Task<bool> IsAdmin(long id)
    {
        return await ef.Accounts.AnyAsync(x => x.IdAccountTelegram == id && x.IsAdmin);
    }

    public async Task Add(Account newAccount)
    {
        if (await ExistAccount(newAccount.IdAccountTelegram))
            return;

        await ef.AddAsync(newAccount);
        await ef.SaveChangesAsync();
        return;
    }

    public async Task<Account?> Get(long idAccount)
    {
        return await ef.Accounts.FirstOrDefaultAsync(x => x.IdAccountTelegram == idAccount);
    }

    public async Task<IEnumerable<Account>> GetAdmins()
    {
        return await ef.Accounts.Where(x => x.IsAdmin).ToListAsync();
    }

    public async Task Update(Account account)
    {
        ef.Update(account);
        await ef.SaveChangesAsync();
    }
}