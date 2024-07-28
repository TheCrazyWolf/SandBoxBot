using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class CheckMessageForMachineLearn(SandBoxRepository repository, ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/check";

    public override async void Execute(Message message)
    {
        if (message.From is null) return;
        
        var props = await repository.Chats.GetByIdAsync(message.Chat.Id);
        
        if (props is null) return;

        if (message.ReplyToMessage is not null)
            message.Text = message.ReplyToMessage.Text;
                
        if (message.Text == null) return;
        
        var isMlNet = message.Text.GetMessageWithoutUserNameBotsAndCommands().IsSpamMl(props.PercentageToDetectSpamFromMl);

        var buildMessage = BuildMessage(  
            isSpamMl : isMlNet.Item1, 
            score: isMlNet.Item2);
        
        TrySendMessage(message.Chat.Id, buildMessage);
    }
    private void TrySendMessage(long idChat, string message)
    {
        try
        {
            botClient.SendTextMessageAsync(chatId: idChat,
                text: message,
                disableNotification: true);
        }
        catch
        {
            // ignored
        }
    }

    private string BuildMessage(bool isSpamMl, float score)
    {
        var resultMessage = isSpamMl 
            ? "\u26a0\ufe0f Похоже, что это является спамом и подлежит блокировке"
            : "\u2705 Похоже, что это обычное сообщение";
        return
            $"\u2705 Команда выполнена" +
            $"\n\nРезультаты распознавания текста: " +
            $"\n\n\u26a1\ufe0f Модель машинного обучения: Вероятность спама {score}%" +
            $"\n\n{resultMessage}" ;
    }
}