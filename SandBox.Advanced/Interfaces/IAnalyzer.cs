using Telegram.Bot.Types;

namespace SandBox.Advanced.Interfaces;

public interface IAnalyzer
{
    void Execute(Message message);
}
