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
        // Если сообщение начинается с '@', то удаляем часть с '@' до пробела
        if (message.StartsWith("@"))
        {
            int spaceIndex = message.IndexOf(' ');
            if (spaceIndex != -1)
            {
                message = message.Substring(spaceIndex + 1);
            }
        }

        // Если сообщение содержит команду с '@', удаляем часть с '@' до пробела
        int atIndex = message.IndexOf('@');
        if (atIndex != -1)
        {
            int spaceIndex = message.IndexOf(' ', atIndex);
            if (spaceIndex == -1)
            {
                message = message.Substring(0, atIndex);
            }
            else
            {
                message = message.Substring(0, atIndex) + message.Substring(spaceIndex);
            }
        }

        return message.Replace($"{UpdateHandler.UserNameBot} ", string.Empty).Trim();
    }

    public static string GetMessageWithoutUserNameBotsAndCommands(string message, int skip = 1)
    {
        var s = message.Replace($"{UpdateHandler.UserNameBot} ", string.Empty).Split(' ').Skip(skip);
        return s.Aggregate(String.Empty, (current, item) => current + $"{item} ");
    }
}