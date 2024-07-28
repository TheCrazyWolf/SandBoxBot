using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Activity;

public class UpdateChatAndUserAfterInvited(SandBoxRepository repository) : IAnalyzer
{
    public async void Execute(Message message)
    {
        if (message.NewChatMembers is null)
            return;

        var chatProps = await repository.Chats.NewChatOrUpdateAsync(message.Chat);

        foreach (var user in message.NewChatMembers)
        {
            await repository.Accounts.NewUserOrUpdateAsync(user);
            await repository.MembersInChat.NewMemberOrUpdateInChatAsync(idChat: message.Chat.Id, user);
        }
    }
}