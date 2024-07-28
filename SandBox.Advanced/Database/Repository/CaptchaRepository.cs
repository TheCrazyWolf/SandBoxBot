using Microsoft.EntityFrameworkCore;
using SandBox.Models.Blackbox;

namespace SandBox.Advanced.Database.Repository;

public class CaptchaRepository(SandBoxContext ef)
{
    public async Task<Captcha> NewCaptchaAsync(Captcha captcha)
    {
        await ef.AddAsync(captcha);
        await ef.SaveChangesAsync();
        return captcha;
    }

    public async Task<Captcha?> GetByIdAsync(long idCaptcha)
    {
        return await ef.Captchas.FirstOrDefaultAsync(x => x.Id == idCaptcha);
    }

    public async Task<bool> ExistsAsync(long idCaptcha)
    {
        return await ef.EventsJoined.AnyAsync(x => x.Id == idCaptcha);
    }

    public async Task<Captcha> UpdateAsync(Captcha captcha)
    {
        ef.Update(captcha);
        await ef.SaveChangesAsync();
        return captcha;
    }

    public async Task<bool> RemoveAsync(long idCaptcha)
    {
        var item = GetByIdAsync(idCaptcha).Result;

        if (item is null)
            return false;

        ef.Remove(item);
        await ef.SaveChangesAsync();

        return true;
    }

    public async void UpdateDecrementAttempAsync(Captcha captha)
    {
        if (captha.AttemptsRemain <= 0)
            return;
        captha.AttemptsRemain--;
        await UpdateAsync(captha);
    }

    public async Task<bool> ContainsCaptchasAsync(long fromId)
    {
        return await ef.Captchas.AnyAsync(x => x.IdTelegram == fromId
                                    && x.DateTimeExpired >= DateTime.Now
                                    && x.AttemptsRemain > 0);
    }
}