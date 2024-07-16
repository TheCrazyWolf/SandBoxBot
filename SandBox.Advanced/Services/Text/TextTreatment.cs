using SandBox.Advanced.Services.Telegram;

namespace SandBox.Advanced.Services.Text;

public static class TextTreatment
{
    public static string GetTrimMessageWithOutUserNameBot(string message)
    {
        int commandIndex = message.IndexOf('/');
        return commandIndex != -1 ? message.Substring(commandIndex).Trim() : message;
    }


    public static List<string> GetArrayWordsTreatmentMessage(string? message)
    {
        if (message is null)
            return [];

        return message.Replace('.', ' ')
            .Replace('-', ' ')
            .Replace(',', ' ')
            .Replace('!', ' ')
            .Replace("\n", " ")
            .Replace('?', ' ')
            .Replace(':', ' ')
            .Replace("  ", " ")
            .Replace(" ", " ")
            .Split(' ')
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();
    }

    public static string GetMessageWithoutUserNameBots(string message)
    {
        return message.Replace($"{UpdateHandler.UserNameBot} ", string.Empty);
    }

    public static string GetMessageWithoutUserNameBotsAndCommands(string message, int skip = 1)
    {
         var s = message.Replace($"{UpdateHandler.UserNameBot} ", string.Empty).Split(' ').Skip(skip);
         return s.Aggregate(String.Empty, (current, item) => current + $"{item} ");
    }
}