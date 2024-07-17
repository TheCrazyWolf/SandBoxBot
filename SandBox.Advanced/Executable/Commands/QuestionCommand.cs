using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Services.Text;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Commands;

public class QuestionCommand: SandBoxHelpers, IExecutable<bool>
{
    private string _blackWords = string.Empty;
    private string _message = string.Empty;
    private IList<Question> _foundedQuestion = default!;

    public Task<bool> Execute()
    {
        if (Update.Message?.From is null)
            return Task.FromResult(false);

        _message = TextTreatment.GetMessageWithoutUserNameBotsAndCommands(Update.Message.Text!);

        _foundedQuestion = Repository.Questions.GetByContentQuestion(_message).Result;
        
        if(_foundedQuestion.Count == 0)
        {
            SendMessage(BuildErrorMessage());
            return Task.FromResult<bool>(true);
        }
        
        SendMessage(BuildSuccessMessage());
        return Task.FromResult(true);
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.

    private IReadOnlyCollection<InlineKeyboardButton> GenerateKeyboardQuestions()
    {
        return _foundedQuestion.Select(item => InlineKeyboardButton.WithCallbackData($"{item.Id}", $"question {item.Id} {Update.Message?.Chat.Id}")).ToList();
    }

    private Task SendMessage(string message)
    {
        var keyboard = GenerateKeyboardQuestions();
        
        BotClient.SendTextMessageAsync(chatId:Update.Message.Chat.Id,
            text: message,
            replyMarkup: new InlineKeyboardMarkup(keyboard), 
            disableNotification: true);
        return Task.CompletedTask;
    }

    private string BuildSuccessMessage()
    {
        var builderMsgQustion = _foundedQuestion
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
    
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}