using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Services.Text;
using SandBox.Models.Blackbox;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class AddNewBlackWord(
    ITelegramBotClient botClient,
    Update update,
    SandBoxRepository repository) : IExecutable
{
    private Account? _accountDb;
    private string _blackWords = string.Empty;

    public Task Execute(CancellationToken cancellationToken)
    {
        if (update.Message?.From is null)
            return Task.CompletedTask;

        _accountDb = repository.Accounts.GetById(update.Message.From.Id).Result;

        if (IfThisUserIsManager().Result)
        {
            Proccess();
            SendMessage(BuildSuccessMessage());
            return Task.CompletedTask;
        }
        
        SendMessage(BuildErrorMessage());
        return Task.CompletedTask;
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
        // проверка, чтобы не добавлялись команды
        var words = TextTreatment.GetArrayWordsTreatmentMessage(update.Message.Text);

        foreach (var word in words)
        {
            repository.BlackWords.Add(new BlackWord { Content = word });
            _blackWords += $"{word}, ";
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
            $"\n\nДобавлены следующие слова: {_blackWords}";
    }
    
    private string BuildErrorMessage()
    {
        return
            "\u26a0\ufe0f Недостаточно прав";
    }
    
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}