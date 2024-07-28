using SandBox.Models.Telegram;

namespace SandBox.Advanced.Utils;

public static class AccountUtils
{
    public static bool IsTrustedProfile(this Account account)
    {
        if (account.IsManagerThisBot)
            return account.IsManagerThisBot;

        if (account.IsGlobalApproved)
            return !account.IsGlobalApproved;

        if (account.IsGlobalRestricted)
            return !account.IsGlobalRestricted;

        return false;
    }
}