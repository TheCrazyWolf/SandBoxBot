using SandBoxBot.Commands.Base;
using SandBoxBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Keyboard.Black;

public class RestoreMessage(ITelegramBotClient botClient, SandBoxRepository repository) : BlackBase(botClient, repository)
{
    public async Task Execute(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var words = callbackQuery.Data?.Split(' ').Skip(1).ToArray();

        if (words is null)
            return;
        
        if (!await ValidateAdmin(callbackQuery.From.Id, callbackQuery.From.Id))
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
                    $"@{account?.UserName}: {incident.Value}. (Восстановленно)", 
                    cancellationToken: cancellationToken);
            
                await BotClient.SendTextMessageAsync(callbackQuery.From.Id, 
                    $"\u2705 Принятые действия по инциденту {incident.Id}: " +
                    $"Восстановлено сообщение",
                    cancellationToken: cancellationToken);
            }
            
        }
        catch (Exception e)
        {
            await BotClient.SendTextMessageAsync(callbackQuery.From.Id, 
                $"🤯 Ошибка восстановления сообщения",
                cancellationToken: cancellationToken);
        }
        
    }
}