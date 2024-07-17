using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Executable.Common;
using SandBox.Models.Events;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Keyboards;

public class QuestionFromDb : EventSandBoxBase, IExecutable<bool>
{
    private Question? _question;

    public Task<bool> Execute()
    {
        if (Update.CallbackQuery?.Data is null)
            return Task.FromResult(false);

        var words = Update.CallbackQuery.Data?.Split(' ').Skip(1).ToArray();

        if (words is null)
            return Task.FromResult(false);

        _question = Repository.Questions.GetById(Convert.ToInt64(words[0])).Result;

        if (_question is null)
            return Task.FromResult(false);

        SendMessageOfExecuted(chatId: Convert.ToInt64(words[1]),
            message: BuildMessage());
        
        return Task.FromResult(true);
    }

    private void SendMessageOfExecuted(long chatId, string message)
    {
        BotClient.SendTextMessageAsync(chatId: chatId,
            text: message,
            disableNotification: true);
    }

    private string BuildMessage()
    {
        return
            $"\u2753 {_question?.Quest}\n\n\u26a1\ufe0f {_question?.Answer}";
    }
}