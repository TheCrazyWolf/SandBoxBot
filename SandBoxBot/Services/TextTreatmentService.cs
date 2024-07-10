namespace SandBoxBot.Services;

public static class TextTreatmentService
{
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