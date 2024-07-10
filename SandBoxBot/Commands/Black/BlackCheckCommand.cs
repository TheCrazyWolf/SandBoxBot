using SandBoxBot.Commands.Base;
using SandBoxBot.Database;
using SandBoxBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackCheckCommand : BlackBase, ICommand
{
    public async Task Execute(Message message, CancellationToken cancellationToken)
    {
        if (message.Text is null)
            return;
        
        var wordArray = TextTreatmentService.GetArrayWordsTreatmentMessage(message.Text);
        
        string blackWords = wordArray.Where(word => Repository.Words.IsContainsWord(word).Result)
            .Aggregate("", (current, word) => current + $"{word} ");

        string verdict = string.IsNullOrEmpty(blackWords)
            ? "\u2705 Нет запрещенных слов"
            : "\ud83d\uded1 Вероятно сообщение является спамом";
        
        await BotClient.SendTextMessageAsync(message.Chat.Id,
            $"[!] Команда выполнена \n\nРезультат: {verdict} \n\nОпознанные слова для блокировки: {blackWords}",
            cancellationToken: cancellationToken);
    }

    public BlackCheckCommand(ITelegramBotClient botClient, SandBoxRepository repository) : base(botClient, repository)
    {
    }
}