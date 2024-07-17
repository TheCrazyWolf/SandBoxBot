using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Services.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class SetMeManager : IExecutable<bool>
{
    public ITelegramBotClient BotClient = default!;
    public Update Update = default!;
    public SandBoxRepository Repository = default!;
    public string Secret = default!;

    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);

        var treatmentText = TextTreatment.GetMessageWithoutUserNameBotsAndCommands(Update.Message.Text!).Split(' ')
            .First();

        if (string.IsNullOrEmpty(treatmentText))
        {
            SendMessage(idChat:Update.Message!.Chat.Id, message: BuildErrorMessage());
            return Task.FromResult(true);
        }

        if (treatmentText == Secret)
        {
            Proccess();
            SendMessage(idChat:Update.Message!.Chat.Id, message: BuildSuccessMessage());
            return Task.FromResult(true);
        }

        SendMessage(idChat:Update.Message!.Chat.Id, message: BuildErrorMessage());
        return Task.FromResult(false);
    }

    private void Proccess()
    {
        var profile = Repository.Accounts.GetById(Update.Message!.From!.Id).Result;

        if (profile is null)
            return;

        profile.IsAprroved = true;
        profile.IsManagerThisBot = true;

        Repository.Accounts.Update(profile);
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
            $"\n\nВы теперь менеджер этого бота, на вас не действуют ограничения, вы можете управлять словарем и событиями";
    }

    private string BuildErrorMessage()
    {
        return
            "\u26a0\ufe0f Секрет неверный";
    }
}