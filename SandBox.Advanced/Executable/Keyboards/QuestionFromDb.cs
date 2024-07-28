using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Executable.Keyboards;

public class QuestionFromDb(SandBoxRepository repository, ITelegramBotClient botClient) : CallQuery
{
    public override string Name { get; set; } = "question";

    public override void Execute(CallbackQuery callbackQuery)
    {
        var words = TryGetArrayFromCallBack(callbackQuery);

        if (words is null) return;

        var quest = repository.Questions.GetByIdAsync(Convert.ToInt64(words[0])).Result;

        if (quest is null)
            return;

        var message = BuildMessage(quest);

        SendMessageOfExecuted(chatId: Convert.ToInt64(words[1]),
            message: message);
    }

    private string[]? TryGetArrayFromCallBack(CallbackQuery callbackQuery)
    {
        try
        {
            return callbackQuery.Data?.Split(' ').Skip(1).ToArray();
        }
        catch
        {
            return null;
        }
    }

    private void SendMessageOfExecuted(long chatId, string message)
    {
        botClient.SendTextMessageAsync(chatId: chatId,
            text: message,
            disableNotification: true);
    }

    private string BuildMessage(Question question)
    {
        return
            $"\u2753 {question.Quest}\n\n\u26a1\ufe0f {question.Answer}";
    }
}