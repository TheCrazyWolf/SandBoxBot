using Microsoft.EntityFrameworkCore;

namespace SandBoxBot.Database.Repository;

public class AdminRepository (SandBoxContext ef)
{
    public async Task<bool> IsAdmin(long idTelegram)
        => await ef.Admins.AnyAsync(x => x.IdTelegram == idTelegram);
    
    public async Task<IReadOnlyList<long>> GetAdminsIds() =>
        await ef.Admins.Select(x => x.IdTelegram).ToListAsync();
}