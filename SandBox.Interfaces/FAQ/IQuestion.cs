using SandBox.Interfaces.Common;

namespace SandBox.Interfaces.FAQ;

public interface IQuestion : IEntity
{
    public string Quest { get; set; }
    public string Answer { get; set; }
}