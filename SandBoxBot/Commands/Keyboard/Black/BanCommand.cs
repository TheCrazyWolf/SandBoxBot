using SandBoxBot.Commands.Base;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Keyboard.Black;

public class BanCommand(ITelegramBotClient botClient, SandBoxRepository repository) : BlackBase(botClient, repository)
{
    public async Task Execute(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var words = callbackQuery.Data?.Split(' ').Skip(1).ToArray();

        if (words is null)
            return;
        
        if (!await ValidateAdmin(callbackQuery.From.Id, callbackQuery.From.Id))
            return;

        try
        {
            await BotClient.BanChatMemberAsync(chatId: Convert.ToInt64(words[0]),
                userId: Convert.ToInt64(words[1]),
                cancellationToken: cancellationToken);
            
            await BotClient.SendTextMessageAsync(callbackQuery.From.Id, $"\u2705 Принятые действия: Пользователь заблокирован",
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            await BotClient.SendTextMessageAsync(callbackQuery.From.Id, $"🤯 Ошибка блокировки пользователя \n\n{e.Message}",
                cancellationToken: cancellationToken);
        }
        
    }
}