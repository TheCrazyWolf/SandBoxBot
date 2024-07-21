using SandBox.Advanced.Interfaces;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Abstract;

public abstract class CallQuery : ICallQuery
{
    public abstract string Name { get; set; }

    public abstract void Execute(CallbackQuery callbackQuery);

    public bool Contains(CallbackQuery callbackQuery)
        => callbackQuery is { Data: not null } && callbackQuery.Data.Contains(Name);
}