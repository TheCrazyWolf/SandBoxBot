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
}