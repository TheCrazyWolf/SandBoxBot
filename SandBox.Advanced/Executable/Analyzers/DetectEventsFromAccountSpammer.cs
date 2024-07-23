using SandBox.Advanced.Database;
using SandBox.Advanced.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Analyzers;

public class DetectEventsFromAccountSpammer(SandBoxRepository repository, ITelegramBotClient botClient) : IAnalyzer
{
    public void Execute(Message message)
    {
        if (message.From is null)
            return;

        var account = repository.Accounts.GetById(message.From.Id).Result;

        if (account != null && account.IsSpamer)
        {
            botClient.DeleteMessageAsync(chatId: message.Chat.Id,
                messageId: message.MessageId);
        }

        return;
    }
}