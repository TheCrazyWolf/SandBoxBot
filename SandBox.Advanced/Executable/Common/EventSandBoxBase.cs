using SandBox.Advanced.Database;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Common;

public class EventSandBoxBase
{
    public SandBoxRepository Repository = default!;
    public ITelegramBotClient BotClient = default!;
    public Update Update = default!;
    public Account? AccountDb = default!;
}