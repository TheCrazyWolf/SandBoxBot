using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Activity;

public sealed class UpdateChatAndDetailsUser(SandBoxRepository repository) : IAnalyzer
{
    public async void Execute(Message message)
    {
        if (message.From?.Id is null)
            return;

        await repository.Chats.UpdateAsync(message.Chat);
        await repository.MembersInChat.UpdateAsync(idChat: message.Chat.Id, user: message.From);
        await repository.Accounts.NewUserOrUpdateAsync(message.From);
    }
}