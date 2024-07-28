using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using SandBox.Advanced.Utils;
using SandBox.Advanced.Utils.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Activity;

public sealed class UpdateChatAndDetailsUser(SandBoxRepository repository, ITelegramBotClient botClient) : IAnalyzer
{
    public async void Execute(Message message)
    {
        if (message.From?.Id is null)
            return;

        var account = await repository.Accounts.NewUserOrUpdateAsync(message.From);

        await repository.Chats.NewChatOrUpdateAsync(message.Chat);

        var memberInChat =
            await repository.MembersInChat.NewMemberOrUpdateInChatAsync(idChat: message.Chat.Id, user: message.From);

        await repository.MembersInChat.UpdateIsAdmin(memberInChat,
            botClient.IsUserAdminInChat(chatId: message.Chat.Id, userId: message.From.Id));

        if (account.IsTrustedProfile())
        {
            repository.Accounts.UpdateApprovedAsync(account);
            return;
        }

        var totalCountSpam = await repository.Contents.CountMessageFromUser(userId: message.From.Id, isSpam: true);
        var totalCountNoSpam = await repository.Contents.CountMessageFromUser(userId: message.From.Id, isSpam: false);

        switch (totalCountNoSpam)
        {
            case >= 3 and <= 29:
                repository.MembersInChat.UpdateAprrovedAsync(memberInChat);
                return;
            case >= 30:
                repository.Accounts.UpdateApprovedAsync(account);
                return;
        }

        switch (totalCountSpam)
        {
            case >= 3 and <= 8:
                repository.MembersInChat.UpdateRestrictedAsync(memberInChat);
                return;
            case >= 9:
                repository.Accounts.UpdateRestrictedAsync(account);
                return;
        }
    }
}