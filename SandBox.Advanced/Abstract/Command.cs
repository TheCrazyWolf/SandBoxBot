using SandBox.Advanced.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SandBox.Advanced.Abstract;

public abstract class Command : ICommand
{
    public abstract string Name { get; set; }

    public abstract void Execute(Message message);

    public bool Contains(Message message)
        => message is { Text: not null, Type: MessageType.Text } && message.Text.Contains(Name);
}