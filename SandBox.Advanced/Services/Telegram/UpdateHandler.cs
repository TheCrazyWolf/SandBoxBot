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
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace SandBox.Advanced.Services.Telegram;

public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger, IServiceScopeFactory scopeFactory) : IUpdateHandler
{
    public static string UserNameBot = string.Empty;
    private static bool _isFirstPool = true;
    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogInformation("HandleError: {Exception}", exception);
        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (_isFirstPool)
        {
            UserNameBot = $"@{botClient.GetMeAsync(cancellationToken).Result.Username}";
            _isFirstPool = false;
        }
        
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message }                        => OnMessage(update),
            { EditedMessage: { } message }                  => OnMessage(update),
            { CallbackQuery: { } callbackQuery }            => OnCallbackQuery(callbackQuery),
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            _                                               => UnknownUpdateHandlerAsync(update)
        });
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // to DO LOG
        throw new NotImplementedException();
    }

    private async Task OnMessage(Update update)
    {
        using var scope = scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<SandBoxRepository>();
        
        logger.LogInformation("Receive message type: {MessageType}", update.Type);
        
        if(update.Message?.NewChatMembers is not null)
            await new UpdateDetailsActivityOnJoined(bot, update, repo).Execute();
        
        if (update.Message?.Text is not { } messageText)
            return;
        
        await new UpdateDetailsActivityProfile(bot, update, repo).Execute();

        var command = TextTreatment.GetMessageWithoutUserNameBots(messageText).Split(' ')[0];

        if (command == "/add")
            await new AddNewBlackWord(bot, update, repo).Execute();
        if (command == "/del")
            await new RemoveBlackWord(bot, update, repo).Execute();
        if (command == "/check")
            await new CheckForBlackWord(bot, update, repo).Execute();

        await new DetectBlackWords(bot, update, repo).Execute();
        await new DetectFastActivity(bot, update, repo).Execute();
		await new DetectSpamMl(bot, update, repo).Execute();

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
        return await bot.SendTextMessageAsync(msg.Chat, usage, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());
    }
    
    static Task<Message> FailingHandler(Message msg)
    {
        throw new NotImplementedException("FailingHandler");
    }

    // Process Inline Keyboard callback data
    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
       
        using var scope = scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<SandBoxRepository>();
        
        var words = callbackQuery.Data?.Split(' ');
        
        if(words is null)
            return;
        
        if(words[0] == "spamrestore")
            await new RestoreFromEvent(bot, callbackQuery, repo).Execute();
        if(words[0] == "spamban")
            await new BanFromEvent(bot, callbackQuery, repo).Execute();
        if(words[0] == "spamnospam")
            await new NoSpamFromEvent(bot, callbackQuery, repo).Execute();
        
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}
