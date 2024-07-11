using System.Windows.Input;
using SandBoxBot.Commands.Base;
using SandBoxBot.Commands.Base.Callback;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Keyboard.Black;

public class BanCommand(ITelegramBotClient botClient, SandBoxRepository repository, CallbackQuery callbackQuery) 
    : EventCallbackQueryCommand(botClient, repository, callbackQuery)
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        var words = CallbackQuery.Data?.Split(' ').Skip(1).ToArray();

        if (words is null)
            return;
        
        if (!await ValidateAdmin(CallbackQuery.From.Id, CallbackQuery.From.Id))
            return;

        try
        {
            await BotClient.BanChatMemberAsync(chatId: Convert.ToInt64(words[0]),
                userId: Convert.ToInt64(words[1]),
                cancellationToken: cancellationToken);
            
            await BotClient.SendTextMessageAsync(CallbackQuery.From.Id, $"\u2705 Принятые действия: Пользователь заблокирован",
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            await BotClient.SendTextMessageAsync(CallbackQuery.From.Id, $"🤯 Ошибка блокировки пользователя \n\n{e.Message}",
                cancellationToken: cancellationToken);
        }
        
    }
}