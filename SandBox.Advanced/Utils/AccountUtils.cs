using SandBox.Advanced.Services.Telegram;
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

        if (account.IsGlobalApproved)
            return !account.IsGlobalApproved;

        if (account.IsGlobalRestricted) 
            return !account.IsGlobalRestricted;
        
        if ((DateTime.Now.Date - account.DateTimeJoined.Date).TotalDays >= 5)
            return true;

        return false;
    }
    
}