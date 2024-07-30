using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using SandBox.Models.Events;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectSpamMachineLearn(SandBoxRepository repository, ITelegramBotClient botClient) : IAnalyzer
{
    public async void Execute(Message message)
    {
        if (message.From is null || string.IsNullOrEmpty(message.Text) || message.Chat.Type is ChatType.Private) return;

        var props = await repository.Chats.GetByIdAsync(message.Chat.Id);

        if (props is null || props.PercentageToDetectSpamFromMl is 0) return;

        var account = await repository.Accounts.GetByIdAsync(message.From.Id);

        var member = await repository.MembersInChat.GetByIdAsync(idChat: message.Chat.Id,
            idTelegram: message.From.Id);

        if (member is null || account is null) return;

        var isToBlock = message.Text.IsSpamMl(props.PercentageToDetectSpamFromMl);

        var @event = message.GenerateEventFromContent(isToBlock.Item1);

        @event.IsSpam = isToBlock.Item1 || member.IsRestricted || account.IsGlobalRestricted;

        if (account.IsGlobalRestricted && member.IsApproved && member.IdChat == message.Chat.Id)
            @event.IsSpam = false; // локальная амнистия (т.е. в текущем чате)
        else if(account.IsGlobalRestricted)
            @event.IsSpam = true;
        else if (member.IsTrustedMember() || account.IsTrustedProfile())
        {
            repository.MembersInChat.UpdateAprrovedAsync(member);
            @event.IsSpam = false;
        }

        _ = repository.Contents.UpdateAsync(@event);

        if (!@event.IsSpam) return;

        TryDeleteMessage(message);
        TryNotifyManagers(message, isToBlock.Item2, GenerateKeyboardForNotify(@event));
        TryKickMember(message, props.AutoKickIfWillBeDetectedSpam);
    }

    private async void TryDeleteMessage(Message originalMessage)
    {
        try
        {
            await botClient.DeleteMessageAsync(chatId: originalMessage.Chat.Id,
                messageId: originalMessage.MessageId);
        }
        catch
        {
            // ignored
        }
    }

    private async void TryKickMember(Message originalMessage, bool isPropAutoKick)
    {
        if (!isPropAutoKick)
            return;

        try
        {
            if (originalMessage.From != null)
                await botClient.UnbanChatMemberAsync(chatId: originalMessage.Chat.Id,
                    userId: originalMessage.From.Id);
        }
        catch
        {
            // ignored
        }
    }

    private async void TryNotifyManagers(Message originalMessage, float score,
        IList<IList<InlineKeyboardButton>> keyboardButtons)
    {
        var memberAdminThisChat = repository.MembersInChat.GetAdminsFromChat(originalMessage.Chat.Id).Result
            .Select(x => Convert.ToInt64(x.IdTelegram));

        var memberManagers = repository.Accounts.GetManagersAsync().Result
            .Select(x => x.IdTelegram);

        var combinedMembers = memberAdminThisChat.Union(memberManagers).ToList();

        foreach (var id in combinedMembers)
        {
            try
            {
                var message = BuildNotifyMessage(originalMessage, score);

                await botClient.SendTextMessageAsync(chatId: id,
                    text: message,
                    replyMarkup: new InlineKeyboardMarkup(keyboardButtons),
                    disableNotification: true);
            }
            catch
            {
                // ignored
            }
        }
    }

    private IList<IList<InlineKeyboardButton>> GenerateKeyboardForNotify(EventContent eventContent)
    {
        return new List<IList<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData("\ud83d\udd39 Восстановить",
                    $"restoremsg {eventContent.ChatId} {eventContent.IdTelegram} {eventContent.Id}"),
                InlineKeyboardButton.WithCallbackData("\u267b\ufe0f Это не спам",
                    $"nospam {eventContent.ChatId} {eventContent.IdTelegram} {eventContent.Id}")
            },

            new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData("\ud83e\udea0 Кик",
                    $"kick {eventContent.ChatId} {eventContent.IdTelegram} {eventContent.Id}"),
                InlineKeyboardButton.WithCallbackData("\ud83e\ude93 Бан",
                    $"ban {eventContent.ChatId} {eventContent.IdTelegram} {eventContent.Id}")
            },
        };
    }

    private string BuildNotifyMessage(Message message, float score)
    {
        return
            $"ℹ️ Я удалил сообщение от юзера \nID # {message.From?.Id}, @{message.From?.Username}\nв чате \nID # {message.Chat.Id} [{message.Chat.Title ?? message.Chat.FirstName}]" +
            $"\n\n{message.Text} \n\nℹ️ Вероятность спама {Math.Round(score, 2)}%";
    }
}