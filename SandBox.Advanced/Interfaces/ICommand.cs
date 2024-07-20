using Telegram.Bot.Types;

namespace SandBox.Advanced.Interfaces;

public interface ICommand
{
    public string Name { get; set; }

    public void Execute(Message message);

    public bool Contains(Message message);
}