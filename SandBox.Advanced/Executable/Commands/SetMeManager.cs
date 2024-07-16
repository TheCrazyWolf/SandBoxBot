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
            SendMessageUnsuccess();
            return Task.FromResult(true);
        }

        if (treatmentText == Secret)
        {
            Proccess();
            SendMessageSuccess();
            return Task.FromResult(true);
        }

        SendMessageUnsuccess();
        return Task.FromResult(false);
    }

    private Task Proccess()
    {
        var profile = Repository.Accounts.GetById(Update.Message!.From!.Id).Result;

        if (profile is null)
            return Task.CompletedTask;

        profile.IsAprroved = true;
        profile.IsManagerThisBot = true;

        Repository.Accounts.Update(profile);
        return Task.CompletedTask;
    }

    private Task SendMessageSuccess()
    {
        var message = BuildSuccessMessage();

        BotClient.SendTextMessageAsync(chatId: Update.Message!.Chat.Id,
            text: message,
            disableNotification: true);
        return Task.CompletedTask;
    }

    private Task SendMessageUnsuccess()
    {
        var message = BuildErrorMessage();

        BotClient.SendTextMessageAsync(chatId: Update.Message!.Chat.Id,
            text: message,
            disableNotification: true);
        return Task.CompletedTask;
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