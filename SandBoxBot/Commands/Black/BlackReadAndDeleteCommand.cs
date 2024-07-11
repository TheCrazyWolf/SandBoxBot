using SandBoxBot.Commands.Base;
using SandBoxBot.Commands.Base.Interfaces;
using SandBoxBot.Commands.Base.Messages;
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
    
    public async Task Execute(CancellationToken cancellationToken)
    {
        if (Message?.From is null || Message.Text is null)
            return;
        
        await UpdateActivityUser(Message.From, Message.Chat.Id);
        
        _isCanBypass = await CanBypass(Message.From!.Id);
        
        string blackWords = string.Empty;

        var wordArray = TextTreatmentService.GetArrayWordsTreatmentMessage(Message.Text);

        _toDelete = false;

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

            await NotifyAdmin(Message, blackWords, incident.Id);
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
                $"\ud83d\udc7e Удалено сообщение от пользователя {message.From?.Id} ({message.From?.Username}) со " +
                $"следующем содержанием: \n\n{message.Text} \n\nЗапрещенные слова: {blackWords} ",
                replyMarkup: new InlineKeyboardMarkup(buttons));
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

        if (account is null) return false;
        
        if (account.IsAdmin)
            return true;
            
        if ((DateTime.Now.Date - account.DateTimeJoined.Date).TotalDays >= 2)
            return true;

        return false;
    }
}