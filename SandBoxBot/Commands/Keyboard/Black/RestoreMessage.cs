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

        string message = words.Skip(1).Aggregate(string.Empty, (current, word) => current + $"{word} ");

        try
        {
            var incident = await Repository.Incidents.Get(Convert.ToInt64(message));

            if (incident is not null)
            {
                incident.IsSpam = false;
                await Repository.Incidents.Update(incident);
                var account = await Repository.Accounts.Get(incident.IdAccountTelegram ?? 0);
                
                await BotClient.SendTextMessageAsync(chatId: Convert.ToInt64(words[0]), 
                    $"\ud83d\uddd3 @{account?.UserName} (Восстановленно): {incident.Value}", 
                    cancellationToken: cancellationToken);
            
                await BotClient.SendTextMessageAsync(CallbackQuery.From.Id, 
                    $"\u2705 Принятые действия по инциденту № {incident.Id}: " +
                    $"Восстановлено сообщение",
                    cancellationToken: cancellationToken);
            }
            
        }
        catch (Exception e)
        {
            await BotClient.SendTextMessageAsync(CallbackQuery.From.Id, 
                $"🤯 Ошибка восстановления сообщения\n\n{e.Message}",
                cancellationToken: cancellationToken);
        }
        
    }
}