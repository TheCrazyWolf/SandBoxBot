using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Services.Text;
using Telegram.Bot;

namespace SandBox.Advanced.Executable.Commands;

public class RemoveBlackWord : SandBoxHelpers, IExecutable<bool>
{
    private string _blackWords = string.Empty;
    private string _message = string.Empty;

    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);

        _message = TextTreatment.GetMessageWithoutUserNameBotsAndCommands(Update.Message.Text!);
        AccountDb = Repository.Accounts.GetById(Update.Message.From.Id).Result;

        if (IfThisUserIsManager(Update.Message.From.Id, Update.Message.Chat.Id).Result)
        {
            Proccess();
            SendMessage(idChat: Update.Message.Chat.Id, message: BuildSuccessMessage());
            return Task.FromResult(true);
        }

        SendMessage(idChat: Update.Message.Chat.Id, message: BuildErrorMessage());
        return Task.FromResult(true);
    }

    private void Proccess()
    {
        // проверка, чтобы не добавлялись команды - SKIP 1
        var words = TextTreatment.GetArrayWordsTreatmentMessage(_message);

        foreach (var word in words.Where(word => Repository.BlackWords.Delete(word).Result))
        {
            _blackWords += $"\ud83d\udd05 {word}\n";
        }
    }

    private void SendMessage(long idChat, string message)
    {
        BotClient.SendTextMessageAsync(chatId: idChat,
            text: message,
            disableNotification: true);
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
}