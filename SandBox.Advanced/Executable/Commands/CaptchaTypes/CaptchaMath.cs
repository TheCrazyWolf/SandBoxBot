using SandBox.Advanced.Executable.Commands.CaptchaTypes.Common;
using SandBox.Models.Blackbox;

namespace SandBox.Advanced.Executable.Commands.CaptchaTypes;

public class CaptchaMath : CaptchaBase
{
    public override string Message { get; set; } = "Решите математическую задачку: ";

    private int FirstValue { get; set; } 
    private int SecondValue { get; set; } 

    public override CaptchaResult Generate(long idTelegram, int lifeTimeMinutes = 1, byte maxAttempts = 1)
    {
        var captchaResult = new CaptchaResult
        {
            CatchaToDb = CreateEntity(idTelegram, lifeTimeMinutes, maxAttempts),
            Message = $"{Message} {FirstValue} сложить {SecondValue} ?" ,
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
        FirstValue = Random.Next(-15, 20);
        SecondValue = Random.Next(-15, 20);
        rightAnswer = (FirstValue + SecondValue).ToString();
        
        for (int i = 0; i < 4; i++)
        {
            listAnswers.Add(Random.Next(FirstValue - 10, FirstValue + 10).ToString());
        }

        listAnswers.Add(rightAnswer);
        return ShuffleAnswers(listAnswers);
    }
}