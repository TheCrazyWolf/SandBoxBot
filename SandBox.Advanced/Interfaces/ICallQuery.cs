using Telegram.Bot.Types;

namespace SandBox.Advanced.Interfaces;

public interface ICallQuery
{
    string Name { get; set; }

    void Execute(CallbackQuery callbackQuery);

    bool Contains(CallbackQuery callbackQuery);
}