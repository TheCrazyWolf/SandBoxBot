using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Utils;
using SandBox.Models.Blackbox;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class AddNewBlackWord(SandBoxRepository repository, ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/add";

    public override void Execute(Message message)
    {
        if (message.From is null)
            return;

        message.Text = message.Text?.GetMessageWithoutUserNameBotsAndCommands();

        var account = repository.Accounts.GetByIdAsync(message.From.Id).Result;

        if (account != null && account.IfUserManager())
        {
            repository.Accounts.UpdateApprovedAsync(account);
            var blockedWords = DoBlockWords(message.Text ?? string.Empty);
            SendMessage(message.Chat.Id, BuildSuccessMessage(blockedWords));
            return;
        }

        SendMessage(message.Chat.Id, BuildErrorMessage());
    }

    private string DoBlockWords(string message)
    {
        var list = string.Empty;
        foreach (var word in message.GetArrayWordsTreatmentMessage(0))
        {
            repository.BlackWords.Add(new BlackWord { Content = word });
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

    private string BuildSuccessMessage(string blockedWords)
    {
        return
            $"\u2705 Команда выполнена" +
            $"\n\nВ черный список добавлены следующие слова: \n\n{blockedWords}";
    }

    private string BuildErrorMessage()
    {
        return
            "\u26a0\ufe0f Недостаточно прав";
    }
}