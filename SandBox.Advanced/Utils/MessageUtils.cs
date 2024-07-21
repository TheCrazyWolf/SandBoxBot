using SandBox_Advanced;
using SandBox.Advanced.Configs;
using SandBox.Advanced.Services.Telegram;

namespace SandBox.Advanced.Utils;

public static class MessageUtils
{
    public static string GetMessageWithoutUserNameBotsAndCommands(this string message, int skip = 1)
    {
        var s = message.Replace($"{BotConfiguration.UserNameBot} ", string.Empty).Split(' ').Skip(skip);
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
    
    public static (bool, float) IsSpamMl(this string? message)
    {
        // model training  lbfgsmaximumEntropyMulti
        
        var sampleData = new AntiWorkSpam.ModelInput
        {
            Value = message ?? string.Empty,
        };
        var result = AntiWorkSpam.Predict(sampleData);

        return result.Score[1] >= UpdateHandler.Configuration.MaxPercentageMachineLearnToBlock ? (true, result.Score[1] * 100) : (false, result.Score[1] * 100);
    }
}