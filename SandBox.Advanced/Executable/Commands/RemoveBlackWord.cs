using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class RemoveBlackWord(SandBoxRepository repository, ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/del";
    public override void Execute(Message message)
    {
        if (message.From is null)
            return;

        message.Text = message.Text?.GetMessageWithoutUserNameBotsAndCommands();

        var account = repository.Accounts.GetById(message.From.Id).Result;

        if (account != null && account.IfUserManager())
        {
            repository.Accounts.UpdateApproved(account);
            var blockedWords = DoUnBlockWords(message.Text ?? string.Empty);
            SendMessage(message.From.Id, BuildSuccessMessage(blockedWords));
            return;
        }

        SendMessage(message.From.Id, BuildErrorMessage());
    }
    
    
    private string DoUnBlockWords(string message)
    {
        var list = string.Empty;
        foreach (var word in message.GetArrayWordsTreatmentMessage(skip: 1)
                     .Where(word => repository.BlackWords.Exists(word).Result))
        {
            repository.BlackWords.Delete(word);
            list += $"\ud83d\udd05 {word}\n";
        }

        return list;
    }

    private void SendMessage(long idChat, string message)
    {
        botClient.SendTextMessageAsync(chatId: idChat,
            text: message,
            disableNotification: true);
    }

    private string BuildSuccessMessage(string unBlockedWords)
    {
        return
            $"\u2705 Команда выполнена" +
            $"\n\nИз черного списка удалены следующие слова: \n\n{unBlockedWords}";
    }

    private string BuildErrorMessage()
    {
        return
            "\u26a0\ufe0f Недостаточно прав";
    }
}