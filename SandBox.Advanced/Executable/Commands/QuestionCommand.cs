using SandBox.Advanced.Abstract;
using SandBox.Advanced.Executable.Common;
using SandBox.Advanced.Services.Text;
using SandBox.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Executable.Commands;

public class QuestionCommand: SandBoxHelpers, IExecutable<bool>
{
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
            SendMessage(idChat: Update.Message.Chat.Id,
                message: BuildErrorMessage());
            return Task.FromResult(true);
        }
        
        SendMessage(idChat: Update.Message.Chat.Id,
            message: BuildSuccessMessage());
        return Task.FromResult(true);
    }

    private IReadOnlyCollection<InlineKeyboardButton> GenerateKeyboardQuestions()
    {
        return _foundedQuestion.Select(item => InlineKeyboardButton.WithCallbackData($"{item.Id}", $"question {item.Id} {Update.Message?.Chat.Id}")).ToList();
    }

    private void SendMessage(long idChat, string message)
    {
        var keyboard = GenerateKeyboardQuestions();
        
        BotClient.SendTextMessageAsync(chatId:idChat,
            text: message,
            replyMarkup: new InlineKeyboardMarkup(keyboard), 
            disableNotification: true);
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
    
}