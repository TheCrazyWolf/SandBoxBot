using Microsoft.Extensions.Options;
using SandBox.Advanced.Abstract;
using SandBox.Advanced.Configs;
using SandBox.Advanced.Database;
using SandBox.Advanced.Executable.Activity;
using SandBox.Advanced.Executable.Analyzers;
using SandBox.Advanced.Executable.Commands;
using SandBox.Advanced.Executable.Keyboards;
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
    private IList<Command> _commands = new List<Command>();

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
        CreateScopeAndGetCurrentService();
        ConfiguringCommands();

        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(update),
            { EditedMessage: { } message } => OnMessage(update),
            { CallbackQuery: { } callbackQuery } => OnCallbackQuery(update),
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

        if (update.Message?.NewChatMembers is not null)
            await new UpdateDetailsActivityOnJoined { BotClient = bot, Update = update, Repository = _repository }
                .Execute();

        if (update.Message?.Text is not { } messageText)
            return;

        await new UpdateDetailsActivityProfile { BotClient = bot, Update = update, Repository = _repository }.Execute();

        // CHECK AND TO DO COMMANDS
        foreach (var command in _commands)
        {
            if (!command.Contains(update.Message))
                continue;
            command.Execute(update.Message);
            return;
        }

        // Переместить выше если будут обходить путем команд
        if (_configuration is { IsChatInWorkTime: true })
        {
            await new DetectNonWorkingTime { BotClient = bot, Update = update, Repository = _repository, }.Execute();
            WorkTimeChatTimer.BotClient = bot;
            WorkTimeChatTimer.Rune();
        }
        // это читай выше 


        await new DetectQuestion { BotClient = bot, Update = update, Repository = _repository, }.Execute();

        // Analatics chats

        switch (_configuration)
        {
            case { IsBlockByMachineLearn: true }:
            {
                if (!await new DetectSpamMl { BotClient = bot, Update = update, Repository = _repository, }.Execute())
                    if (_configuration is { IsBlockByKeywords: true })
                        await new DetectBlackWords { BotClient = bot, Update = update, Repository = _repository, }
                            .Execute();
                break;
            }
            case { IsBlockByKeywords: true }:
                await new DetectBlackWords() { BotClient = bot, Update = update, Repository = _repository, }.Execute();
                break;
        }

        if (_configuration is { IsBlockFastActivity: true })
            await new DetectFastActivity { BotClient = bot, Update = update, Repository = _repository, }.Execute();

        if (_configuration is { IsBlockAntiArab: true })
            await new DetectAntiArab { BotClient = bot, Update = update, Repository = _repository, }.Execute();
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
    private async Task OnCallbackQuery(Update update)
    {
        logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", update.Id);

        var words = update.CallbackQuery?.Data?.Split(' ');

        if (words is null)
            return;

        if (words[0] == "question")
            await new QuestionFromDb { BotClient = bot, Repository = _repository, Update = update }.Execute();
        if (words[0] == "spamrestore")
            await new RestoreFromEvent { BotClient = bot, Repository = _repository, Update = update }.Execute();
        if (words[0] == "spamban")
            await new BanFromEvent { BotClient = bot, Repository = _repository, Update = update }.Execute();
        if (words[0] == "spamnospam")
            await new NoSpamFromEvent { BotClient = bot, Repository = _repository, Update = update }.Execute();
        if (words[0] == "captcha")
            await new CaptchaFromChat { BotClient = bot, Repository = _repository, Update = update }.Execute();
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

    private void CreateScopeAndGetCurrentService()
    {
        var scope = scopeFactory.CreateScope();
        _repository = scope.ServiceProvider.GetRequiredService<SandBoxRepository>();
        _configuration = scope.ServiceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
    }

    private void ConfiguringCommands()
    {
        _commands = new List<Command>()
        {
            new AddNewBlackWord(_repository, bot),
            new CaptchaCommand(_repository, bot)
            // ETC
        };
    }
}