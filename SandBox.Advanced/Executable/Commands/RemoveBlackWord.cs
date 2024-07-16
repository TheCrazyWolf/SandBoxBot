using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Services.Text;
using SandBox.Models.Blackbox;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class RemoveBlackWord(
    ITelegramBotClient botClient,
    Update update,
    SandBoxRepository repository) : IExecutable<bool>
{
    private Account? _accountDb;
    private string _blackWords = string.Empty;
    private string _message = string.Empty;

    public Task<bool> Execute()
    {
        if (update.Message?.From is null)
            return Task.FromResult(false);

        _message = TextTreatment.GetMessageWithoutUserNameBotsAndCommands(update.Message.Text!);
        _accountDb = repository.Accounts.GetById(update.Message.From.Id).Result;

        if (IfThisUserIsManager().Result)
        {
            Proccess();
            SendMessage(BuildSuccessMessage());
            return Task.FromResult(true);
        }
        
        SendMessage(BuildErrorMessage());
        return Task.FromResult(true);
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
    private Task<bool> IfThisUserIsManager()
    {
        if (_accountDb.IsManagerThisBot)
            return Task.FromResult(_accountDb.IsManagerThisBot);

        // to do проверка на администратор ли в беседе

        return Task.FromResult(false);
    }

    private Task Proccess()
    {
        // проверка, чтобы не добавлялись команды - SKIP 1
        var words = TextTreatment.GetArrayWordsTreatmentMessage(_message);

        foreach (var word in words)
        {
            if(repository.BlackWords.Delete(word).Result)
                _blackWords += $"\ud83d\udd05 {word}\n";
        }

        return Task.CompletedTask;
    }

    private Task SendMessage(string message)
    {
        botClient.SendTextMessageAsync(chatId:update.Message.Chat.Id,
            text: message,
            disableNotification: true);
        return Task.CompletedTask;
    }

    private string BuildSuccessMessage()
    {
        return
            $"\u2705 Команда выполнена" +
            $"\n\nИз черного списка удалены следующие слова: \n\n{_blackWords}";
    }
    
    private string BuildErrorMessage()
    {
        return
            "\u26a0\ufe0f Недостаточно прав";
    }
    
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}