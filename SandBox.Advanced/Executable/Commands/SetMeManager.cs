using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Services.Telegram;
using SandBox.Advanced.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class SetMeManager(SandBoxRepository repository, ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/setadmin";

    public override void Execute(Message message)
    {
        if (message.From is null)
            return;

        message.Text = message.Text?.GetMessageWithoutUserNameBotsAndCommands().Split(' ').First();

        var account = repository.Accounts.GetByIdAsync(message.From.Id).Result;
        
        if (string.IsNullOrEmpty(message.Text))
        {
            SendMessage(idChat: message.Chat.Id, message: BuildErrorMessage());
            return;
        }

        if (message.Text == UpdateHandler.Configuration.ManagerPasswordSecret && account != null)
        {
            repository.Accounts.UpdateAdmin(account);
            SendMessage(idChat: message.Chat.Id, message: BuildSuccessMessage());
            return;
        }

        SendMessage(idChat: message.Chat.Id, message: BuildErrorMessage());
    }
    

    private void SendMessage(long idChat, string message)
    {
        botClient.SendTextMessageAsync(chatId: idChat,
            text: message,
            disableNotification: true);
    }

    private string BuildSuccessMessage()
    {
        return
            $"\u2705 Команда выполнена" +
            $"\n\nВы теперь менеджер этого бота, на вас не действуют ограничения, вы можете управлять словарем и событиями";
    }

    private string BuildErrorMessage()
    {
        return
            "\u26a0\ufe0f Секрет неверный";
    }
}