using SandBox.Advanced.Executable.Commands.CaptchaTypes.Common;
using SandBox.Models.Blackbox;

namespace SandBox.Advanced.Executable.Commands.CaptchaTypes;

public class CaptchaImNotBot : CaptchaBase
{
    public override string Message { get; set; } = "Нажмите кнопку ниже";

    public override CaptchaResult Generate(long idTelegram, int lifeTimeMinutes = 1, byte maxAttempts = 1)
    {
        var captchaResult = new CaptchaResult
        {
            CatchaToDb = CreateEntity(idTelegram, lifeTimeMinutes, maxAttempts),
            Message = Message,
            Answers = (List<string>)GetRandom(out var rightAnswer)
        };
        captchaResult.CatchaToDb.Content = rightAnswer;
        return captchaResult;
    }

    private Captcha CreateEntity(long idTelegram, int lifeTimeMinutes = 1, byte maxAttempts = 1)
    {
        return new Captcha
        {
            IdTelegram = idTelegram,
            DateTimeExpired = DateTime.Now.AddMinutes(lifeTimeMinutes),
            AttemptsRemain = maxAttempts,
        };
    }

    private IList<string> GetRandom(out string rightAnswer)
    {
        var listAnswers = new List<string>();
        listAnswers.Add("\u2705");
        rightAnswer = "\u2705";
        return ShuffleAnswers(listAnswers);
    }
}