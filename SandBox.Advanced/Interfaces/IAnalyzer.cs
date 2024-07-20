using Telegram.Bot.Types;

namespace SandBox.Advanced.Interfaces;

public interface IAnalyzer
{
    bool Execute(Message message);
}
