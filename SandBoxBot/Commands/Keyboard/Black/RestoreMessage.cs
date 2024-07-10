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

        string message = string.Empty;

        foreach (var word in words.Skip(1))
        {
            message += $"{word} ";
        }
        
        try
        {
            var incident = await Repository.Incidents.Get(Convert.ToInt64(message));
            incident!.IsSpam = false;
            
            await Repository.Incidents.Update(incident);

            var account = await Repository.Accounts.Get(incident.IdAccountTelegram ?? 0);
            
            await BotClient.SendTextMessageAsync(chatId: Convert.ToInt64(words[0]), 
                $"Восстановлено сообщение от пользователя: @{account?.UserName}: {incident.Value}", cancellationToken: cancellationToken);
            
            await BotClient.SendTextMessageAsync(callbackQuery.From.Id, $"\u2705 Принятые действия: Восстановлено сообщение",
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            await BotClient.SendTextMessageAsync(callbackQuery.From.Id, $"🤯 Ошибка восстановления сообщения",
                cancellationToken: cancellationToken);
        }
        
    }
}