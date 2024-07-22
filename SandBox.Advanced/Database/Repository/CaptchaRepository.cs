using SandBox.Models.Blackbox;

namespace SandBox.Advanced.Database.Repository;

public class CaptchaRepository(SandBoxContext ef)
{
    public Task<Captcha> Add(Captcha captcha)
    {
        ef.Add(captcha);
        ef.SaveChanges();
        return Task.FromResult(captcha);
    }

    public Task<Captcha?> GetById(long idCaptcha)
    {
        return Task.FromResult(ef.Captchas.FirstOrDefault(x => x.Id == idCaptcha));
    }

    public Task<bool> Exists(long idCaptcha)
    {
        return Task.FromResult(ef.EventsJoined.Any(x => x.Id == idCaptcha));
    }

    public Task<Captcha> Update(Captcha captcha)
    {
        ef.Update(captcha);
        ef.SaveChanges();
        return Task.FromResult(captcha);
    }

    public Task<bool> Delete(long idCaptcha)
    {
        var item = GetById(idCaptcha).Result;

        if (item is null)
            return Task.FromResult(false);

        ef.Remove(item);
        ef.SaveChanges();

        return Task.FromResult(true);
    }

    public void UpdateDecrementAttemp(Captcha captha)
    {
        if (captha.AttemptsRemain <= 0)
            return;
        captha.AttemptsRemain--;
        Update(captha);
    }

    public bool ContainsCaptchas(long fromId)
    {
        return ef.Captchas.Any(x => x.IdTelegram == fromId
                                    && x.DateTimeExpired >= DateTime.Now
                                    && x.AttemptsRemain > 0);
    }
}