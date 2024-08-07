using System.Text.RegularExpressions;
using SandBox_Advanced;
using SandBox.Advanced.Configs;
using SandBox.Advanced.Services.Telegram;
using AntiWorkSpam = SandBox.Advanced.MachineLearn.AntiWorkSpam;

namespace SandBox.Advanced.Utils;

public static class MessageUtils
{
    public static string GetMessageWithoutUserNameBotsAndCommands(this string message, int skip = 1)
    {
        var s = message.Replace($"{BotConfiguration.BotInfo.Username} ", string.Empty).Split(' ').Skip(skip);
        return s.Aggregate(String.Empty, (current, item) => current + $"{item} ");
    }
    
    public static List<string> GetArrayWordsTreatmentMessage(this string? message, int skip = 0)
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
            .Skip(skip)
            .ToList();
    }
    
    public static string GetMessageForFaq(this string message)
    {
        var newMessage = message.Replace("Здравствуйте", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("доброе утро", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("добрый день", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("добрый вечер", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("доброй ночи", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (newMessage.StartsWith("!") || newMessage.StartsWith(".") || newMessage.StartsWith(",") || newMessage.StartsWith(" ") || newMessage.StartsWith("\n"))
        {
            newMessage = newMessage.Substring(1);
        }

        return newMessage;
    }
    
    
    public static bool IsContaintsUrls(this string message)
    {
        string linkPattern = @"((http|https):\/\/)?[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,}(\/\S*)?";
        return Regex.IsMatch(message, linkPattern);
    }
    
    public static (bool, float) IsSpamMl(this string? message, float percentage)
    {
        // model training  lbfgsmaximumEntropyMulti
        
        var sampleData = new AntiWorkSpam.ModelInput
        {
            Value = message ?? string.Empty,
        };
        var result = AntiWorkSpam.Predict(sampleData);

        return result.Score[1] >= percentage ? (true, result.Score[1] * 100) : (false, result.Score[1] * 100);
    }
}