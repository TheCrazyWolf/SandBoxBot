using SandBoxBot.Commands.Base;
using SandBoxBot.Database;
using SandBoxBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackReadAndDeleteCommand : BlackBase, ICommand
{
    public async Task Execute(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        if (message.Text is null)
            return;

        string blackWords = string.Empty;
        
        var wordArray = GetArrayWordsTreatmentMessage(message.Text);

        bool toDelete = false;

        foreach (var word in wordArray)
        {
            if (!await Repository.Words.IsContainsWord(word)) continue;

            toDelete = true;
            blackWords += $"{word} ";
        }
        
        var sentence = new Sentence
        {
            Value = message.Text,
            IsSpam = toDelete
        };

        await Repository.Sentences.Add(sentence);
        
        if (toDelete)
        {
            await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId,
                cancellationToken: cancellationToken);

            foreach (var id in await Repository.Admins.GetAdminsIds())
            {
                await botClient.SendTextMessageAsync(id,
                    $"[!] Удалено сообщение от пользователя {message.From?.Id} ({message.From?.Username}) со следующем содержанием: \n\n{message.Text} \n\nЗапрещенные слова: {blackWords}",
                    cancellationToken: cancellationToken);
            }
        }
    }

    public BlackReadAndDeleteCommand(ITelegramBotClient botClient, SandBoxRepository repository) : base(botClient, repository)
    {
    }
}