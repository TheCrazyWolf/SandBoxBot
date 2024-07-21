using SandBox.Advanced.Executable.Commands.CaptchaTypes.Common;
using SandBox.Models.Blackbox;

namespace SandBox.Advanced.Executable.Commands.CaptchaTypes;

public class CaptchaMath : CaptchaBase
{
    public override string Message { get; set; } = "Решите математическую задачку: ";

    private int _firstValue;
    private int _secondValue;

    public override CaptchaResult Generate(long idTelegram, int lifeTimeMinutes = 1, byte maxAttempts = 1)
    {
        Random rnd = new Random();
        _firstValue = rnd.Next(0, 20);
        _secondValue = rnd.Next(0, 20);
        var captchaResult = new CaptchaResult
        {
            CatchaToDb = CreateEntity(idTelegram, lifeTimeMinutes, maxAttempts),
            Message = $"{Message} {_firstValue} сложить {_secondValue} ?" ,
            Answers = (List<string>)GetRandom(out var rightAnswer)
        };
        captchaResult.CatchaToDb.Content = rightAnswer;

        return captchaResult;
    }

    private Captcha CreateEntity(long idTelegram, int lifeTimeMinutes = 1, byte maxAttempts = 2)
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
        rightAnswer = (_firstValue + _secondValue).ToString();
        
        for (int i = 0; i < 4; i++)
        {
            listAnswers.Add(Random.Next(_firstValue - 10, _firstValue + 10).ToString());
        }

        listAnswers.Add(rightAnswer);
        return ShuffleAnswers(listAnswers);
    }
}