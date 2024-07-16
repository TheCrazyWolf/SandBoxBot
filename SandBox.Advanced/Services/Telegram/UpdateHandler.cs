using Microsoft.Extensions.Options;
using SandBox.Advanced.Configs;
using SandBox.Advanced.Database;
using SandBox.Advanced.Executable.Activity;
using SandBox.Advanced.Executable.Analyzers;
using SandBox.Advanced.Executable.Commands;
using SandBox.Advanced.Executable.Keyboards;
using SandBox.Advanced.Services.Text;
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
    public static string UserNameBot = string.Empty;
    private static bool _isFirstPool = true;
    private SandBoxRepository _repository = default!;
    private BotConfiguration _configuration = default!;

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
        await CreateScopeAndGetCurrentService();

        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(update),
            { EditedMessage: { } message } => OnMessage(update),
            { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
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

        if (update.Message?.NewChatMembers is not null)
            await new UpdateDetailsActivityOnJoined(bot, update, _repository!).Execute();

        if (update.Message?.Text is not { } messageText)
            return;

        await new UpdateDetailsActivityProfile(bot, update, _repository!).Execute();

        var command = TextTreatment.GetMessageWithoutUserNameBots(messageText).Split(' ')[0];

        switch (command.ToLower())
        {
            case "/add":
                await new AddNewBlackWord(bot, update, _repository!).Execute();
                break;
            case "/del":
                await new RemoveBlackWord(bot, update, _repository!).Execute();
                break;
            case "/start":
                await new StartCommand(bot, update, _repository!).Execute();
                break;
            case "/check":
                await new CheckForBlackWord(bot, update, _repository!).Execute();
                return;
            case "/setadmin":
                await new SetMeManager
                {
                    BotClient = bot,
                    Update = update,
                    Repository = _repository,
                    Secret = _configuration!.ManagerPasswordSecret,
                }.Execute();
                break;
        }

        // Analatics chats

        switch (_configuration)
        {
            case { IsBlockByMachineLearn: true }:
            {
                if (!await new DetectSpamMl(bot, update, _repository!).Execute())
                    if (_configuration is { IsBlockByKeywords: true })
                        await new DetectBlackWords(bot, update, _repository!).Execute();
                break;
            }
            case { IsBlockByKeywords: true }:
                await new DetectBlackWords(bot, update, _repository!).Execute();
                break;
        }

        if (_configuration is { IsBlockFastActivity: true })
            await new DetectFastActivity(bot, update, _repository!).Execute();
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

        if (words[0] == "spamrestore")
            await new RestoreFromEvent(bot, callbackQuery, _repository!).Execute();
        if (words[0] == "spamban")
            await new BanFromEvent(bot, callbackQuery, _repository!).Execute();
        if (words[0] == "spamnospam")
            await new NoSpamFromEvent(bot, callbackQuery, _repository!).Execute();
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    private Task GetUserNameBotIfFirstPoll()
    {
        if (!_isFirstPool) return Task.CompletedTask;

        UserNameBot = $"@{bot.GetMeAsync().Result.Username}";
        _isFirstPool = false;
        return Task.CompletedTask;
    }

    private Task CreateScopeAndGetCurrentService()
    {
        var scope = scopeFactory.CreateScope();
        _repository = scope.ServiceProvider.GetRequiredService<SandBoxRepository>();
        _configuration = scope.ServiceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        return Task.CompletedTask;
    }
}