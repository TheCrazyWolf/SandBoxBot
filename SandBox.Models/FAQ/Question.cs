using SandBox.Interfaces.FAQ;
using SandBox.Models.Common;

namespace SandBox.Models.FAQ;

public class Question : Entity, IQuestion
{
    public string Quest { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}