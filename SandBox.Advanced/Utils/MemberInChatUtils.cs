using SandBox.Models.Members;

namespace SandBox.Advanced.Utils;

public static class MemberInChatUtils
{
    public static bool IsTrustedMember(this MemberChat memberChat)
    {
        if (memberChat.IsAdmin)
            return memberChat.IsAdmin;
        
        if (memberChat.IsApproved)
            return memberChat.IsApproved;

        if ((DateTime.Now.Date - memberChat.DateTimeJoined.Date).TotalDays >= 5)
            return true;

        return false;
    }
}