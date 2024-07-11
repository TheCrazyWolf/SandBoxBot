using SandBoxBot.Commands.Base;
using SandBoxBot.Commands.Base.Interfaces;
using SandBoxBot.Commands.Base.Messages;
using SandBoxBot.Configs;
using SandBoxBot.Database;
using SandBoxBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBoxBot.Commands.Black;

public class BlackCheckCommand(ITelegramBotClient botClient, SandBoxRepository repository, Message? message)
    : EventMessageCommand(botClient, repository, message), ICommand
{
    private LevinshtainService _levinshtainService = new();

#pragma warning disable CS0414 // Field is assigned but its value is never used
    private bool _isDetectedLevinshtain = false;
    private bool _isDetectedWordInBlac = false;
#pragma warning restore CS0414 // Field is assigned but its value is never used

    public async Task Execute(CancellationToken cancellationToken)
    {
        if (Message?.Text is null)
            return;

        var wordArray = TextTreatmentService.GetArrayWordsTreatmentMessage(Message.Text);

        string blackWords = wordArray.Where(word => Repository.Words
                .IsContainsWord(word).Result)
            .Aggregate("", (current, word) => current + $"{word} ");

        _isDetectedWordInBlac = !string.IsNullOrEmpty(blackWords);
        _isDetectedLevinshtain = await _levinshtainService.IsSpamAsync(Message.Text, GlobalConfigs.DistanceLevinsthain);

        string verdict = !_isDetectedLevinshtain && !_isDetectedWordInBlac
            ? "\u2705 Нет запрещенных слов"
            : "\ud83d\uded1 Вероятно сообщение является спамом";

        string strBuildLevin = _isDetectedLevinshtain ? "Распознано как спам" : "Не распознано как спам";

        await BotClient.SendTextMessageAsync(Message.Chat.Id,
            $"\u2705 Команда выполнена \n\n{verdict}" +
            $"\n\nОпознанные слова для блокировки: {blackWords}\n\nАлгоритм Левинштейна: {strBuildLevin}",
            disableNotification: true,
            cancellationToken: cancellationToken);
    }
}