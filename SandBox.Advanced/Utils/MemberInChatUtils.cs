using SandBox.Models.Telegram;

namespace SandBox.Advanced.Utils;

public static class MemberInChatUtils
{
    public static bool IsTrustedMember(this MemberInChat memberInChat)
    {
        if (memberInChat.IsApproved)
            return memberInChat.IsApproved;

        if (memberInChat.IsAdmin)
            return memberInChat.IsAdmin;

        if ((DateTime.Now.Date - memberInChat.DateTimeJoined.Date).TotalDays >= 5)
            return true;

        return false;
    }
}