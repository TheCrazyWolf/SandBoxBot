using SandBox.Advanced.Executable.Commands.CaptchaTypes.Common;
using SandBox.Models.Blackbox;

namespace SandBox.Advanced.Executable.Commands.CaptchaTypes;

public class CaptchaEmoji : CaptchaBase
{
    private readonly IDictionary<string, string> _emoji = new Dictionary<string, string>()
    {
        { "🍏", "🍎" }, { "🤡", "💩" }, { "☠️", "👺" }, { "😛", "🍎" },
        { "🤖", "🎃" }, { "😳️", "🤯" }, { "👾", "😇" }, { "💥", "⚡️" },
        { "💦", "❄️" }, { "⛈", "🌤" }, { "🥥", "🥝" }
    };

    public override string Message { get; set; } = "Выберите отличающийся эмодзи";

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
        var random = _emoji.Skip(Random.Next(0, _emoji.Count)).First();
        var listAnswers = new List<string>();
        
        for (int i = 0; i < 4; i++)
        {
            listAnswers.Add(random.Key);
        }
        listAnswers.Add(random.Value);
        rightAnswer = random.Value;
        return ShuffleAnswers(listAnswers);
    }
}