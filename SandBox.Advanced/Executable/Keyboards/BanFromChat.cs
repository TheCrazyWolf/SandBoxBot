using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Interfaces;
using SandBox.Models.Events;
using Telegram.Bot;

namespace SandBox.Advanced.Executable.Keyboards;

public class BanFromChat : SandBoxHelpers, IExecutable<bool>
{
    private EventContent? _eventContent;

    public Task<bool> Execute()
    {
        if (Update.CallbackQuery is null)
            return Task.FromResult(false);
        
        var words = Update.CallbackQuery.Data?.Split(' ').Skip(1).ToArray();

        if (words is null)
            return Task.FromResult(false);

        _eventContent = Repository.Contents.GetById(Convert.ToInt64(words[0])).Result;

        if (_eventContent is null)
            return Task.FromResult(false);
        
        AccountDb = Repository.Accounts.GetById(Convert.ToInt64(_eventContent.IdTelegram)).Result;
        
        if (AccountDb is not null)
        {
            AccountDb.IsSpamer = true;
            Repository.Accounts.Update(AccountDb);
        }

        Proccess(idChat: Convert.ToInt64(_eventContent.ChatId), idUser: Convert.ToInt64(_eventContent.IdTelegram));
        SendMessageOfExecuted(idChat: Update.CallbackQuery.From.Id, message: BuildNotifyMessage());
        return Task.FromResult(true);
    }

    private void Proccess(long idChat, long idUser)
    {
        BotClient.BanChatMemberAsync(chatId: idChat,
            userId: Convert.ToInt64(idUser));
    }

    private void SendMessageOfExecuted(long idChat, string message)
    {
        BotClient.SendTextMessageAsync(chatId: idChat,
            text: message,
            disableNotification: true);
    }

    private string BuildNotifyMessage()
    {
        return
            $"\u2705 Принятые действия: Пользователь заблокирован";
    }
}