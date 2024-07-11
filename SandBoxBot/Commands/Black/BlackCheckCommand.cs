using SandBoxBot.Commands.Base;
using SandBoxBot.Commands.Base.Interfaces;
using SandBoxBot.Commands.Base.Messages;
using SandBoxBot.Database;
using SandBoxBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackCheckCommand(ITelegramBotClient botClient, SandBoxRepository repository, Message? message)
    : EventMessageCommand(botClient, repository, message), ICommand
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        if (Message?.Text is null)
            return;
        
        var wordArray = TextTreatmentService.GetArrayWordsTreatmentMessage(Message.Text);
        
        string blackWords = wordArray.Where(word => Repository.Words.IsContainsWord(word).Result)
            .Aggregate("", (current, word) => current + $"{word} ");

        string verdict = string.IsNullOrEmpty(blackWords)
            ? "\u2705 Нет запрещенных слов"
            : "\ud83d\uded1 Вероятно сообщение является спамом";
        
        await BotClient.SendTextMessageAsync(Message.Chat.Id,
            $"\u2705 Команда выполнена \n\n{verdict}" +
            $"\n\nОпознанные слова для блокировки: {blackWords}",
            cancellationToken: cancellationToken);
    }
    
}