using Microsoft.EntityFrameworkCore;
using SandBoxBot.Database;
using SandBoxBot.Models;

namespace SandBoxBot.Commands.Base;

public class BlackBase
{
    protected bool IsAdmin(long idTelegram)
        => BlackBoxContext.Instance.Admins.Any(x => x.IdTelegram == idTelegram);

    protected bool IsContainsWord(string valueWord)
        => BlackBoxContext.Instance.BlackWords.Any(x => x.Word == valueWord);

    protected async Task Add(string newWord)
    {
        if (IsContainsWord(newWord))
            return;

        var blackWord = new BlackWord()
        {
            Word = newWord
        };
        
        await BlackBoxContext.Instance.AddAsync(blackWord);
        await BlackBoxContext.Instance.SaveChangesAsync();
    }
    
    protected async Task Delete(string word)
    {
        if(!IsContainsWord(word))
            return;

        var foundWord = await BlackBoxContext.Instance.BlackWords
            .FirstOrDefaultAsync(x => x.Word == word);
        
        if(foundWord is null)
            return;

        BlackBoxContext.Instance.Remove(foundWord);
        await BlackBoxContext.Instance.SaveChangesAsync();
    }

    protected async Task<IReadOnlyList<long>> GetAdminsIds() =>
        await BlackBoxContext.Instance.Admins.Select(x => x.IdTelegram).ToListAsync();


    protected string[] GetArrayWordsTreatmentMessage(string message)
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