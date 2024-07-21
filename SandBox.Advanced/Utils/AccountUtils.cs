using SandBox.Models.Telegram;

namespace SandBox.Advanced.Utils;

public static class AccountUtils
{
    public static bool IfUserManager(this Account account)
    {
        return account.IsManagerThisBot;
    }

    public static bool IsTrustedProfile(this Account account)
    {
        if (account.IsManagerThisBot)
            return account.IsManagerThisBot;

        if (account.IsNeedToVerifyByCaptcha)
            return !account.IsNeedToVerifyByCaptcha;

        if (account.IsAprroved) // Прошедший капчу
            return account.IsAprroved;

        if (account.IsSpamer)
            return !account.IsSpamer;

        // Доверенный профиль, вероятность того что профиль на забанят через 4 дня после спама минимальная ?
        if ((DateTime.Now.Date - account.DateTimeJoined.Date).TotalDays >= 4)
            return true;

        return false;
    }
    
}