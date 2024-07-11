using SandBoxBot.Database;

namespace SandBoxBot.Services;

public class LevinshtainService
{
    private readonly SandBoxRepository _repository = new(SandBoxContext.Instance);

    // Функция расстояния Левенштейна
    public int LevenshteinDistance(string s1, string s2)
    {
        int n = s1.Length;
        int m = s2.Length;
        int[,] d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; d[i, 0] = i++) { }
        for (int j = 0; j <= m; d[0, j] = j++) { }

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }
        return d[n, m];
    }

    // Метод для проверки спама на основе Левенштейна
    public async Task<bool> IsSpamAsync(string message, int maxDistance = 2)
    {
        var blackSentences = await _repository.Incidents.GetAll(); 

        foreach (var blackSentence in blackSentences)
        {
            if (LevenshteinDistance(message, blackSentence.Value) <= maxDistance)
            {
                return true;
            }
        }
        return false;
    }
}