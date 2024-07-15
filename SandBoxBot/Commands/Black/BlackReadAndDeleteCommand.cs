using SandBoxBot.Commands.Base.Interfaces;
using SandBoxBot.Commands.Base.Messages;
using SandBoxBot.Configs;
using SandBoxBot.Database;
using SandBoxBot.Models;
using SandBoxBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBoxBot.Commands.Black;

public class BlackReadAndDeleteCommand(ITelegramBotClient botClient, SandBoxRepository repository, Message? message)
    : EventMessageCommand(botClient, repository, message), ICommand
{
    private bool _isCanBypass;
    private bool _toDelete;
    private bool _isWorkTime;
    private LevinshtainService _levinshtainService = new();

    private static Message? _lastMessageWriteToWorkTime;
    private static DateTime _lastDateTimeWriteToWorkTime;

    public async Task Execute(CancellationToken cancellationToken)
    {
        if (Message?.From is null || Message.Text is null)
            return;

        await UpdateActivityUser();

        /*_isCanBypass = await CanBypass();
        _isWorkTime = CanWriteInWorkTime();

        if (!(_isWorkTime || await CanBypassIsAdmin()))
        {
            await SendAndDeleteMessageIfNotWorkTime();
            return;
        }*/

        string blackWords = string.Empty;

        var wordArray = TextTreatmentService.GetArrayWordsTreatmentMessage(Message.Text);

        foreach (var word in wordArray)
        {
            if (!await Repository.Words.IsContainsWord(word)) continue;

            _toDelete = true;
            blackWords += $"{word} ";
        }

        var sentence = new Incident
        {
            Value = Message.Text,
            IsSpam = _toDelete,
            DateTime = DateTime.Now,
            ChatId = Message.Chat.Id,
            IdAccountTelegram = Message.From?.Id
        };

        if (_isCanBypass)
        {
            sentence.IsSpam = false;
            _toDelete = false;
        }

        var incident = await Repository.Incidents.Add(sentence);

        if (_toDelete)
        {
            await BotClient.DeleteMessageAsync(Message.Chat.Id, Message.MessageId,
                cancellationToken: cancellationToken);

            await NotifyAdmin(blackWords, incident.Id, false);
        }
        else if (GlobalConfigs.IsWorkLevinshtain)
        {
            if (Message?.Text?.Split(' ').Length <= GlobalConfigs.MinimalWordToCheckLevinshtain)
                return;

            var result = await _levinshtainService.IsSpamAsync(Message!.Text, GlobalConfigs.DistanceLevinsthain);

            if (result)
            {
                await BotClient.DeleteMessageAsync(Message.Chat.Id, Message.MessageId,
                    cancellationToken: cancellationToken);

                await NotifyAdmin(blackWords, incident.Id, result);
            }
        }
    }

    private async Task NotifyAdmin(string blackWords, long idIncident, bool isLevinshtain)
    {
        foreach (var id in await Repository.Accounts.GetAdmins())
        {
            var buttons = new InlineKeyboardButton[][]
            {
                [
                    InlineKeyboardButton.WithCallbackData("\ud83d\udd39 Восстановить",
                        $"restore {idIncident}"),
                    InlineKeyboardButton.WithCallbackData("\ud83e\ude93 Забанить юзера",
                        $"ban {idIncident}")
                ],
                [
                    InlineKeyboardButton.WithCallbackData("\u267b\ufe0f Это не спам",
                        $"nospam {idIncident}")
                ]
            };

            string isWorkLevinshtain = GlobalConfigs.IsWorkLevinshtain ? "Да" : "Нет";
            string isDetectLevinshtain = isLevinshtain ? "Да" : "Нет";

            await BotClient.SendTextMessageAsync(id.IdAccountTelegram,
                $"\ud83d\udc7e Удалено сообщение от пользователя {Message?.From?.Id} ({Message?.From?.Username}) со " +
                $"следующем содержанием: \n\n{Message?.Text} \n\nЗапрещенные слова: {blackWords} \n\nАлгоритм Левинштейна работает: {isWorkLevinshtain}\nСообщение удалено по алгоритму: {isDetectLevinshtain}",
                replyMarkup: new InlineKeyboardMarkup(buttons), disableNotification: true);
        }
    }

    private async Task UpdateActivityUser()
    {
        if (Message?.From is null)
            return;

        var account = await Repository.Accounts.Get(Message.From.Id);

        if (account is null)
        {
            account = new Account
            {
                IdAccountTelegram = Message.From.Id,
                ChatId = Message.Chat.Id,
                LastActivity = DateTime.Now,
                DateTimeJoined = DateTime.Now,
                UserName = Message.From.Username,
                FirstName = Message.From.FirstName,
                LastName = Message.From.LastName,
            };

            await Repository.Accounts.Add(account);
        }

        account.LastActivity = DateTime.Now;
        account.UserName = Message.From.Username;
        account.FirstName = Message.From.FirstName;
        account.LastName = Message.From.LastName;
        await Repository.Accounts.Update(account);
    }

    private async Task<bool> CanBypass()
    {
        if (Message?.From is null)
            return false;

        var account = await Repository.Accounts.Get(Message.From.Id);

        if (account is null) return false;

        if (account.IsAdmin)
            return true;

        if ((DateTime.Now.Date - account.DateTimeJoined.Date).TotalDays >= 2)
            return true;

        return false;
    }

    private async Task<bool> CanBypassIsAdmin()
    {
        if (Message?.From is null)
            return false;

        var account = await Repository.Accounts.Get(Message.From.Id);

        if (account is null)
            return false;

        return account.IsAdmin;
    }

    private bool CanWriteInWorkTime()
    {
        if (DateTime.Now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return false;

        var start = new TimeSpan(08, 00, 00);
        var end = new TimeSpan(16, 30, 00);
        var endShort = new TimeSpan(15, 30, 00);

        if (!(DateTime.Now.TimeOfDay >= start && DateTime.Now.TimeOfDay <= end))
            return false;

        if (DateTime.Now.DayOfWeek is DayOfWeek.Friday &&
            (!(DateTime.Now.TimeOfDay >= start && DateTime.Now.TimeOfDay <= endShort)))
            return false;

        return true;
    }

    private async Task SendAndDeleteMessageIfNotWorkTime()
    {
        if (Message is null)
            return;

        try
        {
            var currentTime = DateTime.Now;
            var timeSinceLastMessage = (currentTime - _lastDateTimeWriteToWorkTime).TotalHours;

            // Если прошло больше 8 часов с момента последнего сообщения о рабочем времени
            if (_lastMessageWriteToWorkTime != null && timeSinceLastMessage >= 8)
            {
                await BotClient.DeleteMessageAsync(Message.Chat.Id, _lastMessageWriteToWorkTime.MessageId);
                _lastMessageWriteToWorkTime = null;
            }

            // Отправка сообщения о рабочем времени, если оно еще не было отправлено в последние 8 часов
            if (_lastMessageWriteToWorkTime == null)
            {
                _lastMessageWriteToWorkTime = await BotClient.SendTextMessageAsync(Message.Chat.Id,
                    $"Мы хотим помогать Вам круглосуточно \u2764\ufe0f\nНо получить ответы на вопросы Вы можете в рабочее время: ПН-ЧТ С 8.00 по 16.30, ПТ до 15.30 \u2705",
                    disableNotification: true);
                _lastDateTimeWriteToWorkTime = currentTime;
            }

            // Удаление входящего сообщения пользователя
            await BotClient.DeleteMessageAsync(Message.Chat.Id, Message.MessageId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}