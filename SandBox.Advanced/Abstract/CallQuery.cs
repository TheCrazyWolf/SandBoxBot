using SandBox.Advanced.Interfaces;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Abstract;

public abstract class CallQuery : ICallQuery
{
    public string Name { get; set; } = string.Empty;

    public abstract void Execute(CallbackQuery callbackQuery);

    public bool Contains(CallbackQuery callbackQuery)
        => callbackQuery is { Data: not null } && callbackQuery.Data.Contains(Name);
}