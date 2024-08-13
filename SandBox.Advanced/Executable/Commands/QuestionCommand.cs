using SandBox.Advanced.Abstract;
using SandBox.Advanced.Database;
using SandBox.Advanced.Utils;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Commands;

public class QuestionCommand(SandBoxRepository repository, ITelegramBotClient botClient) : Command
{
    public override string Name { get; set; } = "/question";

    public override void Execute(Message message)
    {
        if (message.From is null || message.Text is null) return;

        var questions = repository.Questions
            .GetByContentQuestionAsync(message.Text.GetMessageWithoutUserNameBotsAndCommands())
            .Result;

        if (questions.Count is 0)
        {
            TrySendMessage(idChat: message.Chat.Id,
                message: BuildErrorMessage(),
                keyboardButtons: new List<InlineKeyboardButton>());
            return;
        }

        TrySendMessage(idChat: message.Chat.Id,
            message: BuildSuccessMessage(questions),
            keyboardButtons: GenerateKeyboardQuestions(questions, message.Chat.Id));
    }

    private IReadOnlyCollection<InlineKeyboardButton> GenerateKeyboardQuestions(IList<Question> questions, long chatId)
    {
        return questions.Select(item => InlineKeyboardButton
            .WithCallbackData($"{item.Id}", $"question {item.Id} {chatId}")).ToList();
    }

    private void TrySendMessage(long idChat, string message, IReadOnlyCollection<InlineKeyboardButton> keyboardButtons)
    {
        try
        {
            botClient.SendTextMessageAsync(chatId: idChat,
                text: message,
                replyMarkup: new InlineKeyboardMarkup(keyboardButtons),
                disableNotification: true);
        }
        catch 
        {
            // ignored
        }
    }

    private string BuildSuccessMessage(IList<Question> questions)
    {
        var builderMsgQustion = questions
            .Aggregate(string.Empty, (current, item) => current + $"\ud83d\udca5 #{item.Id}. {item.Quest}\n\n");

        return
            $"\u2705 Команда выполнена" +
            $"\n\nВот, какие похожие вопросы задавали ранее и есть на них ответ: \n\n{builderMsgQustion}\nВыберите пожалуйста вопрос, чтобы получить ответ";
    }

    private string BuildErrorMessage()
    {
        return
            "\u26a0\ufe0f К сожалению, мы не нашли в базе знаний ответа на ваш вопрос";
    }
}