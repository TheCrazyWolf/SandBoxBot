using Microsoft.Extensions.Options;
using SandBox.Advanced.Configs;
using SandBox.Advanced.Database;
using SandBox.Advanced.Executable.Activity;
using SandBox.Advanced.Executable.Analyzers;
using SandBox.Advanced.Executable.Commands;
using SandBox.Advanced.Executable.Keyboards;
using SandBox.Advanced.Executable.Services;
using SandBox.Advanced.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace SandBox.Advanced.Services.Telegram;

public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger, IServiceScopeFactory scopeFactory)
    : IUpdateHandler
{
    public static BotConfiguration Configuration { get; set; } = new();

    private static readonly long IdChatMain = -1001941895047; // -1001941895047 приемка, -1002233749723 test
    private static IList<ICommand> _commands = new List<ICommand>();
    private static IList<IAnalyzer> _analyzerActivity = new List<IAnalyzer>();
    private static IList<IAnalyzer> _analyzers = new List<IAnalyzer>();
    private static IList<ICallQuery> _callBackQueryies = new List<ICallQuery>();
    private static IList<IService> _services = new List<IService>();

    private static bool _isFirstPool = true;
    private static SandBoxRepository _repository = default!;

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
        await ConfiguringIfFirstPoll();

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

    private Task OnMessage(Update update)
    {
        logger.LogInformation("Receive message type: {MessageType}", update.Type);

        if (update.EditedMessage is not null)
            update.Message = update.EditedMessage;

        if (update.Message is null)
            return Task.CompletedTask;

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
            return Task.CompletedTask;
        }

        // Первочередные анализаторы (добавление пользователей в бд, заходы в чаты и тд
        foreach (var command in _analyzers)
        {
            command.Execute(update.Message);
        }

        return Task.CompletedTask;
    }
    
    // Process Inline Keyboard callback data
    private Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);

        foreach (var callBack in _callBackQueryies)
        {
            if (!callBack.Contains(callbackQuery))
                continue;
            callBack.Execute(callbackQuery);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    private Task ConfiguringIfFirstPoll()
    {
        if (!_isFirstPool) return Task.CompletedTask;

        _isFirstPool = false;

        var result = bot.GetMeAsync().Result;
        BotConfiguration.UserNameBot = $"@{result.Username}";
        BotConfiguration.IdBot = result.Id;
        CreateScopeAndGetCurrentService();
        ConfiguringCommands();
        ConfiguringAnalyzers();
        ConfiguringActivityAnalyzers();
        ConfiguringCallBackQueryies();
        ConfiguringServices();

        foreach (var service in _services)
        {
            Task.Run(() => service.Execute());
        }

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
            new StartCommand(_repository, bot),
            new TimeCommand(bot),
            new MenuCommand(bot)
            // ETC
        };
    }

    private void ConfiguringActivityAnalyzers()
    {
        _analyzerActivity = new List<IAnalyzer>
        {
            new UpdateDetailsActivityProfile(_repository),
            new UpdateDetailsActivityOnJoined(_repository),
        };
    }


    private void ConfiguringAnalyzers()
    {
        _analyzers = new List<IAnalyzer>()
        {
            new DetectAsyncServerTime(_repository, bot),
            new DetectTelegramBotInChat(_repository, bot, IdChatMain),
            new DetectEventsFromAccountSpammer(_repository, bot),
            new DetectMediaInMessageNonTrusted(_repository, bot, IdChatMain),
            new DetectUrlsInMsgNonTrusted(_repository, bot, IdChatMain),
            new DetectSpamMl(_repository, bot, IdChatMain),
            new DetectBlackWords(_repository, bot, IdChatMain),
            new DetectFastActivityFromUser(_repository, bot, IdChatMain),
            new DetectFastJoins(_repository, bot, IdChatMain),
            new DetectQuestion(_repository, 1946031755, IdChatMain), // 1946031755 - Смирнова, 208049718 - я
        };
    }

    private void ConfiguringCallBackQueryies()
    {
        _callBackQueryies = new List<ICallQuery>()
        {
            new BanFromChat(_repository, bot),
            new BanFromEvent(_repository, bot),
            new CaptchaFromChat(_repository, bot),
            new NoSpamFromEvent(_repository, bot),
            new QuestionFromDb(_repository, bot),
            new RestoreFromEvent(_repository, bot),
        };
    }

    private void ConfiguringServices()
    {
        _services = new List<IService>()
        {
            new WorkTimeChatTimer(_repository, bot, IdChatMain) 
        };
    }
}