using SandBoxBot.Commands.Base;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackCheckCommand : BlackBase, ICommand
{
    public async Task Execute(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        if (message.Text is null)
            return;
        
        var wordArray = GetArrayWordsTreatmentMessage(message.Text.ToLower());
        
        string blackWords = wordArray.Where(word => IsContainsWord(word.ToLower()))
            .Aggregate("", (current, word) => current + $"{word} ");

        string verdict = string.IsNullOrEmpty(blackWords)
            ? "Нет запрещенных слов"
            : "Вероятно сообщение является спамом";
        
        await botClient.SendTextMessageAsync(message.Chat.Id,
            $"[!] Команда выполнена \nРезультат: {verdict} \nОпознанные слова для блокировки: {blackWords}",
            cancellationToken: cancellationToken);
    }
}