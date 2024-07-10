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
        if (message.NewChatMembers is not null)
            await RegisterIfNewUser(message.NewChatMembers, message.Chat.Id);

        if (message.From is not null)
            await UpdateActivityUser(message.From, message.Chat.Id);

        if (message.Text is null)
            return;

        bool canBypass = await CanBypass(message.From!.Id);
        
        string blackWords = string.Empty;

        var wordArray = TextTreatmentService.GetArrayWordsTreatmentMessage(message.Text);

        bool toDelete = false;

        foreach (var word in wordArray)
        {
            if (!await Repository.Words.IsContainsWord(word)) continue;

            toDelete = true;
            blackWords += $"{word} ";
        }

        var sentence = new Incident
        {
            Value = message.Text,
            IsSpam = toDelete,
            DateTime = DateTime.Now,
            IdAccountTelegram = message.From?.Id
        };

        var incident = await Repository.Incidents.Add(sentence);

        if (toDelete)
        {
            await BotClient.DeleteMessageAsync(message.Chat.Id, message.MessageId,
                cancellationToken: cancellationToken);

            await NotifyAdmin(message, blackWords, incident.Id);
        }
    }

    private async Task NotifyAdmin(Message message, string blackWords, long idIncident)
    {
        foreach (var id in await Repository.Accounts.GetAdmins())
        {
            var buttons = new InlineKeyboardButton[][]
            {
                [
                    InlineKeyboardButton.WithCallbackData("\ud83d\udd39 Это не спам",
                        $"restore {message.Chat.Id} {idIncident}"),
                    InlineKeyboardButton.WithCallbackData("\ud83e\ude93 Забанить юзера",
                        $"ban {message.Chat.Id} {message.From?.Id}")
                ]
            };

            await BotClient.SendTextMessageAsync(id.IdAccountTelegram,
                $"[!] Удалено сообщение от пользователя {message.From?.Id} ({message.From?.Username}) со " +
                $"следующем содержанием: \n\n{message.Text} \n\nЗапрещенные слова: {blackWords} ",
                replyMarkup: new InlineKeyboardMarkup(buttons));
        }
    }

    private async Task RegisterIfNewUser(User[] invitedUsers, long chatId)
    {
        foreach (var user in invitedUsers)
        {
            if (await Repository.Accounts.ExistAccount(user.Id))
                continue;

            var newAccount = new Account()
            {
                IdAccountTelegram = user.Id,
                ChatId = chatId,
                LastActivity = DateTime.Now,
                DateTimeJoined = DateTime.Now,
                UserName = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            await Repository.Accounts.Add(newAccount);
        }
    }

    private async Task UpdateActivityUser(User user, long chatId)
    {
        var account = await Repository.Accounts.Get(user.Id);

        if (account is null)
        {
            account = new Account
            {
                IdAccountTelegram = user.Id,
                ChatId = chatId,
                LastActivity = DateTime.Now,
                DateTimeJoined = DateTime.Now,
                UserName = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            await Repository.Accounts.Add(account);
        }

        account.LastActivity = DateTime.Now;
        account.UserName = user.Username;
        account.FirstName = user.FirstName;
        account.LastName = user.LastName;
        await Repository.Accounts.Update(account);
    }

    private async Task<bool> CanBypass(long idAccount)
    {
        var account = await Repository.Accounts.Get(idAccount);
        
        if ((DateTime.Now.Date - account!.DateTimeJoined.Date).TotalDays >= 4)
            return true;

        return account.IsAdmin;
    }

    public BlackReadAndDeleteCommand(ITelegramBotClient botClient, SandBoxRepository repository) : base(botClient,
        repository)
    {
    }
}