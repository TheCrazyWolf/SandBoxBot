using Microsoft.Extensions.Options;
using SandBox.Advanced.Abstract;
using SandBox.Advanced.Configs;
using SandBox.Advanced.Database;
using SandBox.Advanced.Executable.Activity;
using SandBox.Advanced.Executable.Analyzers;
using SandBox.Advanced.Executable.Commands;
using SandBox.Advanced.Executable.Keyboards;
using SandBox.Advanced.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Services.Telegram;

public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger, IServiceScopeFactory scopeFactory)
    : IUpdateHandler
{
    public static BotConfiguration Configuration { get; set; } = new ();
    
    private IList<ICommand> _commands = new List<ICommand>();
    private IList<IAnalyzer> _analyzerActivity = new List<IAnalyzer>();
    private IList<IAnalyzer> _analyzers = new List<IAnalyzer>();
    private IList<ICallQuery> _callBackQueryies = new List<ICallQuery>();
    
    private static bool _isFirstPool = true;
    private SandBoxRepository _repository = default!;

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("HandleError: {Exception}", exception);
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        await GetUserNameBotIfFirstPoll();
        CreateScopeAndGetCurrentService();
        ConfiguringCommands();
        ConfiguringAnalyzers();
        ConfiguringActivityAnalyzers();

        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(update),
            { EditedMessage: { } message } => OnMessage(update),
            { CallbackQuery: { } callbackQuery } => OnCallbackQuery(update.CallbackQuery),
            _ => UnknownUpdateHandlerAsync(update)
        });
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task OnMessage(Update update)
    {
        logger.LogInformation("Receive message type: {MessageType}", update.Type);

        if (update.EditedMessage is not null)
            update.Message = update.EditedMessage;
        
        if (update.Message?.Text is not { } messageText)
            return;
        
        // Первочередные анализаторы (добавление пользователей в бд, заходы в чаты и тд
        foreach (var command in _analyzerActivity)
        {
            command.Execute(update.Message);
        }
        
        // Проверка на наличие команда и исполнение
        foreach (var command in _commands)
        {
            if (!command.Contains(update.Message))
                continue;
            command.Execute(update.Message);
            return;
        }
        
        // Первочередные анализаторы (добавление пользователей в бд, заходы в чаты и тд
        foreach (var command in _analyzers)
        {
            command.Execute(update.Message);
        }
    }

    async Task<Message> Usage(Message msg)
    {
        const string usage = """
                                 <b><u>Bot menu</u></b>:
                                 /photo          - send a photo
                                 /inline_buttons - send inline buttons
                                 /keyboard       - send keyboard buttons
                                 /remove         - remove keyboard buttons
                                 /request        - request location or contact
                                 /inline_mode    - send inline-mode results list
                                 /poll           - send a poll
                                 /poll_anonymous - send an anonymous poll
                                 /throw          - what happens if handler fails
                             """;
        return await bot.SendTextMessageAsync(msg.Chat, usage, parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }

    // Process Inline Keyboard callback data
    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);

        var words = callbackQuery.Data?.Split(' ');

        if (words is null)
            return;

        foreach (var callBack in _callBackQueryies)
        {
            if (!callBack.Contains(callbackQuery))
                continue;
            callBack.Execute(callbackQuery);
            return;
        }

        /*if (words[0] == "question")
            await new QuestionFromDb { BotClient = bot, Repository = _repository, Update = update }.Execute();
        if (words[0] == "spamrestore")
            await new RestoreFromEvent { BotClient = bot, Repository = _repository, Update = update }.Execute();
        if (words[0] == "spamban")
            await new BanFromEvent { BotClient = bot, Repository = _repository, Update = update }.Execute();
        if (words[0] == "spamnospam")
            await new NoSpamFromEvent { BotClient = bot, Repository = _repository, Update = update }.Execute();
        if (words[0] == "captcha")
            await new CaptchaFromChat { BotClient = bot, Repository = _repository, Update = update }.Execute();*/
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    private Task GetUserNameBotIfFirstPoll()
    {
        if (!_isFirstPool) return Task.CompletedTask;

        BotConfiguration.UserNameBot = $"@{bot.GetMeAsync().Result.Username}";
        _isFirstPool = false;
        return Task.CompletedTask;
    }

    private void CreateScopeAndGetCurrentService()
    {
        var scope = scopeFactory.CreateScope();
        _repository = scope.ServiceProvider.GetRequiredService<SandBoxRepository>();
        Configuration = scope.ServiceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
    }

    private void ConfiguringCommands()
    {
        _commands = new List<ICommand>()
        {
            new AddNewBlackWord(_repository, bot),
            new CaptchaCommand(_repository, bot),
            new CheckForBlackWord(_repository, bot),
            new QuestionCommand(_repository, bot),
            new RemoveBlackWord(_repository, bot),
            new SetMeManager(_repository, bot),
            new StartCommand(_repository, bot)
            // ETC
        };
    }
    
    private void ConfiguringActivityAnalyzers()
    {
        _analyzerActivity = new List<IAnalyzer>
        {
            new UpdateDetailsActivityProfile(_repository, bot),
            new UpdateDetailsActivityOnJoined(_repository, bot),
        };
    }
    
    
    private void ConfiguringAnalyzers()
    {
        _analyzers = new List<IAnalyzer>()
        {
            new DetectBlackWords(_repository, bot)
            // ETC
        };
    }
}