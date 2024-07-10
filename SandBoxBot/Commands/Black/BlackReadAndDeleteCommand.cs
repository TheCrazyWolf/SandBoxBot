using SandBoxBot.Commands.Base;
using SandBoxBot.Database;
using SandBoxBot.Models;
using SandBoxBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBoxBot.Commands.Black;

public class BlackReadAndDeleteCommand : BlackBase, ICommand
{
    public async Task Execute(Message message, CancellationToken cancellationToken)
    {
        if (message.Text is null)
            return;

        string blackWords = string.Empty;
        
        var wordArray = TextTreatmentService.GetArrayWordsTreatmentMessage(message.Text);

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

        var incident = await Repository.Sentences.Add(sentence);
        
        if (toDelete)
        {
            await BotClient.DeleteMessageAsync(message.Chat.Id, message.MessageId,
                cancellationToken: cancellationToken);

            await NotifyAdmin(message, blackWords, incident.Id);
        }
    }

    private async Task NotifyAdmin(Message message, string blackWords, long idIncident)
    {
        foreach (var id in await Repository.Admins.GetAdminsIds())
        {
            var buttons = new InlineKeyboardButton[][]
            {
                [
                    InlineKeyboardButton.WithCallbackData("\ud83d\udd39 Это не спам", $"restore {message.Chat.Id} {idIncident}" ),
                    InlineKeyboardButton.WithCallbackData("\ud83e\ude93 Забанить пользователя", $"ban {message.Chat.Id} {message.From?.Id}")
                ]
            };
            
            await BotClient.SendTextMessageAsync(id,
                $"[!] Удалено сообщение от пользователя {message.From?.Id} ({message.From?.Username}) со " +
                $"следующем содержанием: \n\n{message.Text} \n\nЗапрещенные слова: {blackWords} ", 
                replyMarkup: new InlineKeyboardMarkup(buttons));
            
        }
    }
    
    public BlackReadAndDeleteCommand(ITelegramBotClient botClient, SandBoxRepository repository) : base(botClient, repository)
    {
    }
}