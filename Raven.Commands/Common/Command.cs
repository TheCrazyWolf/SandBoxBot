using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Raven.Commands.Common;

public abstract class Command
{
    public abstract string Name { get; }

    public abstract void Execute(Message message);

    public bool Contains(Message message)
        => message is { Text: not null, Type: MessageType.Text } && message.Text.Contains(Name);
}