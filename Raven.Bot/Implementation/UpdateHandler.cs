using Microsoft.Extensions.Options;
using SandBox.Advanced.Configs;
using SandBox.Advanced.Database;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Raven.Bot.Implementation;

public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger, IServiceScopeFactory scopeFactory)
    : IUpdateHandler
{
    public static BotConfiguration Configuration { get; set; } = new();

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

        if (update.EditedMessage is not null) update.Message = update.EditedMessage;

        if (update.Message is null) return Task.CompletedTask;

        return Task.CompletedTask;
    }
    
    // Process Inline Keyboard callback data
    private Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
        
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

        BotConfiguration.BotInfo = bot.GetMeAsync().Result;
        BotConfiguration.BotInfo.Username = $"@{BotConfiguration.BotInfo.Username}";
        
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
        
    }

    private void ConfiguringActivityAnalyzers()
    {
       
    }


    private void ConfiguringAnalyzers()
    {
       
    }

    private void ConfiguringCallBackQueryies()
    {
        
    }
    
}