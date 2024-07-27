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

        await repository.Chats.UpdateAsync(message.Chat);

        var memberInChat = await repository.MembersInChat.UpdateAsync(idChat: message.Chat.Id, user: message.From);

        await repository.MembersInChat.UpdateIsAdmin(memberInChat,
            botClient.IsUserAdminInChat(chatId: message.Chat.Id, userId: message.From.Id));

        var account = await repository.Accounts.NewUserOrUpdateAsync(message.From);

        if (account.IsTrustedProfile())
        {
            repository.Accounts.UpdateApprovedAsync(account);
            return;
        }

        var totalCountSpam = await repository.Contents.CountMessageFromUser(userId: message.From.Id, isSpam: true);
        var totalCountNoSpam = await repository.Contents.CountMessageFromUser(userId: message.From.Id, isSpam: false);

        if (totalCountNoSpam >= 3)
        {
            repository.Accounts.UpdateApprovedAsync(account);
            return;
        }

        if (totalCountSpam >= 5)
        {
            repository.Accounts.UpdateRestrictedAsync(account);
        }
    }
}