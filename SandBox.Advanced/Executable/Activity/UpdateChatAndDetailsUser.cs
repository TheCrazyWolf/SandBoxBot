using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
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
        await repository.Accounts.NewUserOrUpdateAsync(message.From);
    }
}