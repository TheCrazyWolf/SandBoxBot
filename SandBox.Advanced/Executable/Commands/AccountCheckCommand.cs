using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Commands;

public class AccountCheckCommand(SandBoxRepository repository, ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/acheck";

    public override async void Execute(Message message)
    {
        if (message.From is null) return;
        
        var props = await repository.Chats.GetByIdAsync(message.Chat.Id);
        
        if (props is null) return;

        if (message.ReplyToMessage is not null)
            message = message.ReplyToMessage;
                
        if (message.From is null) return;

        var account = await repository.Accounts.GetByIdAsync(message.From.Id);
        
        var memberInChat =
            await repository.MembersInChat.GetByIdAsync(idChat: message.Chat.Id, idTelegram: message.From.Id);
        
        if(account is null || memberInChat is null) return;
        
        TrySendMessage(message.Chat.Id, BuildMessage(originalMessage: message, account: 
            account, memberInChat: memberInChat, props: props));
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

    private string BuildMessage(Message originalMessage, Account account, MemberInChat memberInChat, ChatProps props)
    {
        return
            $"\u2705 Команда выполнена" +
            $"\n\nID аккаунта: {originalMessage.From?.Id}" +
            $"\nUserName: {originalMessage.From?.Username}" +
            $"\nID чата: {originalMessage.Chat.Id}" +
            $"\nПорог срабатывания ML: {props.PercentageToDetectSpamFromMl}" +
            $"\nАвтокик при срабатывании: {ConvertBoolean(props.AutoKickIfWillBeDetectedSpam)}" +
            $"\nID сообщения: {originalMessage.MessageId}\n\n" +
            $"Поведение:\n" +
            $"Глобальные ограничения: {ConvertBoolean(account.IsGlobalRestricted)}\n" +
            $"Глобальные доверие: {ConvertBoolean(account.IsGlobalApproved)}\n\n" +
            $"Локальные ограничения: {ConvertBoolean(memberInChat.IsRestricted)}\n" +
            $"Локальное доверие: {ConvertBoolean(memberInChat.IsApproved)}\n" +
            $"Локальный админ: {ConvertBoolean(memberInChat.IsAdmin)}\n";
    }

    private string ConvertBoolean(bool value)
    {
        return value ? "Да" : "Нет";
    }
}