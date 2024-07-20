namespace SandBox.Advanced.Executable.Commands.CaptchaTypes.Common;

public abstract class CaptchaBase
{
    protected readonly Random Random = new();
    public abstract string Message { get; set; }
    public abstract CaptchaResult Generate(long idTelegram, int lifeTimeMinutes = 1, byte maxAttempts = 1);
    
    protected IList<string> ShuffleAnswers(IList<string> answers)
        => answers.OrderBy(_ => Random.Next()).ToList();
}