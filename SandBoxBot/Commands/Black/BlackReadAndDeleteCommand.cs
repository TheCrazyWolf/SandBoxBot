using SandBoxBot.Commands.Base;
using SandBoxBot.Database;
using SandBoxBot.Models;
using SandBoxBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackReadAndDeleteCommand : BlackBase, ICommand
{
    public async Task Execute(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        if (message.Text is null)
            return;

        string blackWord = string.Empty;

        // преобработка текста
        var wordArray = message.Text.Replace('.', ' ')
            .Replace(',', ' ')
            .Replace('!', ' ')
            .Replace("\n", " ")
            .Replace('?', ' ')
            .Replace(':', ' ')
            .Replace("  ", " ")
            .Replace(" ", " ")
            .Split(' ')
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();

        bool toDelete = false;

        foreach (var word in wordArray)
        {
            if (!IsContainsWord(word.ToLower())) continue;

            toDelete = true;
            blackWord += $"{word} ";
        }
        
        var sentence = new Sentence
        {
            Value = message.Text,
            IsSpam = toDelete
        };

        await BlackBoxContext.Instance.AddAsync(sentence, cancellationToken);
        await BlackBoxContext.Instance.SaveChangesAsync(cancellationToken);
        
        if (toDelete)
        {
            await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId,
                cancellationToken: cancellationToken);

            foreach (var id in await GetAdminsIds())
            {
                await botClient.SendTextMessageAsync(id,
                    $"[!] Удалено сообщение от пользователя {message.From?.Id} ({message.From?.Username}) со следующем содержанием: \n\n{message.Text} \n\nЗапрещенные слова: {blackWord}",
                    cancellationToken: cancellationToken);
            }
        }
    }
}