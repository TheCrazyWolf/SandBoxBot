using SandBoxBot.Commands.Base;
using SandBoxBot.Commands.Base.Callback;
using SandBoxBot.Commands.Base.Messages;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Keyboard.Black;

public class RestoreMessage(ITelegramBotClient botClient, SandBoxRepository repository, CallbackQuery callbackQuery)
    : EventCallbackQueryCommand(botClient, repository, callbackQuery)
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        var words = CallbackQuery.Data?.Split(' ').Skip(1).ToArray();

        if (words is null)
            return;

        if (!await ValidateAdmin(CallbackQuery.From.Id, CallbackQuery.From.Id))
            return;
        
        var incident = await Repository.Incidents.Get(Convert.ToInt64(words[0]));

        if (incident is null)
            return;
        
        try
        {
            incident.IsSpam = false;
            await Repository.Incidents.Update(incident);
            var account = await Repository.Accounts.Get(incident.IdAccountTelegram ?? 0);

            await BotClient.SendTextMessageAsync(chatId: incident.ChatId,
                $"\ud83d\uddd3 @{account?.UserName} (Восстановленно): {incident.Value}",
                cancellationToken: cancellationToken);

            await BotClient.SendTextMessageAsync(CallbackQuery.From.Id,
                $"\u2705 Принятые действия по инциденту № {incident.Id}: " +
                $"Восстановлено сообщение",
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            await BotClient.SendTextMessageAsync(CallbackQuery.From.Id,
                $"🤯 Ошибка восстановления сообщения\n\n{e.Message}",
                cancellationToken: cancellationToken);
        }
    }
}