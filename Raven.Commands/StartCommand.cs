using Raven.Commands.Common;
using Telegram.Bot.Types;

namespace Raven.Commands;

public class StartCommand : Command
{
    public override string Name { get; } = "/start";

    public override void Execute(Message message)
    {
        throw new NotImplementedException();
    }
}