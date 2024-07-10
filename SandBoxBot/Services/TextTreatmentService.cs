using System.Text.RegularExpressions;

namespace SandBoxBot.Services;

public static class TextTreatmentService
{
    public static string GetTrimMessageWithOutUserNameBot(string message)
    {
        int commandIndex = message.IndexOf('/');
        return commandIndex != -1 ? message.Substring(commandIndex).Trim() : message; 
    }


    public static string[] GetArrayWordsTreatmentMessage(string message)
    {
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
            .ToArray();
    }
}