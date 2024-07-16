using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Services.Text;
using SandBox.Models.Blackbox;
using Telegram.Bot;

namespace SandBox.Advanced.Executable.Commands;

public class AddNewBlackWord() : SandBoxHelpers, IExecutable<bool>
{
    private string _blackWords = string.Empty;
    private string _message = string.Empty;

    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);

        _message = TextTreatment.GetMessageWithoutUserNameBotsAndCommands(Update.Message.Text!);

        AccountDb = Repository.Accounts.GetById(Update.Message.From.Id).Result;

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

    private Task Proccess()
    {
        // проверка, чтобы не добавлялись команды - SKIP 1
        var words = TextTreatment.GetArrayWordsTreatmentMessage(_message);

        foreach (var word in words)
        {
            Repository.BlackWords.Add(new BlackWord { Content = word });
            _blackWords += $"\ud83d\udd05 {word}\n";
        }

        return Task.CompletedTask;
    }

    private Task SendMessage(string message)
    {
        BotClient.SendTextMessageAsync(chatId:Update.Message.Chat.Id,
            text: message,
            disableNotification: true);
        return Task.CompletedTask;
    }

    private string BuildSuccessMessage()
    {
        return
            $"\u2705 Команда выполнена" +
            $"\n\nВ черный список добавлены следующие слова: \n\n{_blackWords}";
    }
    
    private string BuildErrorMessage()
    {
        return
            "\u26a0\ufe0f Недостаточно прав";
    }
    
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}