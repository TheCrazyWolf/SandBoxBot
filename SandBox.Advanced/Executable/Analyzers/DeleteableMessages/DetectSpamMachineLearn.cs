using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using SandBox.Models.Events;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Analyzers.DeleteableMessages;

public class DetectSpamMachineLearn(
    SandBoxRepository repository,
    ITelegramBotClient botClient) : IAnalyzer
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

        if (isToBlock.Item1)
            @event.IsSpam = true;

        if (member.IsTrustedMember() || account.IsTrustedProfile())
        {
            repository.MembersInChat.UpdateAprrovedAsync(member);
            @event.IsSpam = false;
        }

        await repository.Contents.UpdateAsync(@event);

        if (!@event.IsSpam) return;

        await botClient.DeleteMessageAsync(chatId: message.Chat.Id,
            messageId: message.MessageId);

        NotifyManagers(message, isToBlock.Item2, GenerateKeyboardForNotify(@event));
    }

    private async void NotifyManagers(Message originalMessage, float score,
        IList<IList<InlineKeyboardButton>> keyboardButtons)
    {
        foreach (var id in await repository.MembersInChat.GetAdminsFromChat(originalMessage.Chat.Id))
        {
            try
            {
                var message = BuildNotifyMessage(originalMessage, score);

                await botClient.SendTextMessageAsync(chatId: id.IdTelegram!,
                    text: message,
                    replyMarkup: new InlineKeyboardMarkup(keyboardButtons),
                    disableNotification: true);
            }
            catch
            {
                // ignored
            }
        }
        
        foreach (var id in await repository.Accounts.GetManagers())
        {
            try
            {
                var message = BuildNotifyMessage(originalMessage, score);

                await botClient.SendTextMessageAsync(chatId: id.IdTelegram!,
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
                    $"spamrestore {eventContent.Id}"),
                InlineKeyboardButton.WithCallbackData("\ud83e\ude93 Забанить юзера",
                    $"spamban {eventContent.Id}")
            },

            new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData("\u267b\ufe0f Это не спам",
                    $"spamnospam {eventContent.Id}")
            },
        };
    }

    private string BuildNotifyMessage(Message message, float score)
    {
        return
            $"\ud83d\udc7e Удалено сообщение от пользователя {message.From?.Id} (@{message.From?.Username}) в чате # {message.Chat.Id} - ({message.Chat.Title ?? message.Chat.FirstName}) со " +
            $"следующем содержанием: \n\n{message.Text} \n\nℹ️ Это сообщение удалено по решению модели машинного обучения. Вероятность спама составила {score}%\n\n";
    }
}