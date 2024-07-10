﻿using SandBoxBot.Commands.Base;
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

        // преобработка текста
        var wordArray = GetArrayWordsTreatmentMessage(message.Text.ToLower());

        bool toDelete = false;

        foreach (var word in wordArray)
        {
            if (!IsContainsWord(word.ToLower())) continue;

            toDelete = true;
            blackWords += $"{word} ";
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
                    $"[!] Удалено сообщение от пользователя {message.From?.Id} ({message.From?.Username}) со следующем содержанием: \n\n{message.Text} \n\nЗапрещенные слова: {blackWords}",
                    cancellationToken: cancellationToken);
            }
        }
    }
}