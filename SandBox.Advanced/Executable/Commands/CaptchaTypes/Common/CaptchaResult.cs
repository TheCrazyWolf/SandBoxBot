using SandBox.Models.Blackbox;

namespace SandBox.Advanced.Executable.Commands.CaptchaTypes.Common;

public class CaptchaResult
{
    public Captcha CatchaToDb { get; set; } = default!;
    public string Message { get; set; } = default!;
    public List<string> Answers { get; set; } = default!;
}