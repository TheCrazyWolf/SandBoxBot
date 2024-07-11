using SandBoxBot.Commands.About;
using SandBoxBot.Commands.Admins;
using SandBoxBot.Commands.Black;
using SandBoxBot.Commands.Keyboard.Black;
using SandBoxBot.Database;
using SandBoxBot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace SandBoxBot.Events.Base;

public class UpdateHandler : IUpdateHandler
{
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var handler = update switch
        {
            // UpdateType.Unknown:
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            // UpdateType.Poll:
            { Message: { } message } => BotOnMessageReceived(botClient, message, cancellationToken),
            { EditedMessage: { } message } => BotOnMessageReceived(botClient, message, cancellationToken),
            { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(botClient, callbackQuery,
                cancellationToken),
            { InlineQuery: { } inlineQuery } => BotOnInlineQueryReceived(botClient, inlineQuery, cancellationToken),
            { ChosenInlineResult: { } chosenInlineResult } => BotOnChosenInlineResultReceived(botClient,
                chosenInlineResult, cancellationToken),
            _ => UnknownUpdateHandlerAsync(botClient, update, cancellationToken)
        };

        //await handler;
    }

    private async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (message.NewChatMembers is not null)
        {
            await Task.Run(()=> new WelcomeMessageCommand(botClient, new(SandBoxContext.Instance))
                .Execute(message, cancellationToken), cancellationToken);
        }

        if (message.Text is not { } messageText)
            return;

        var messageTreatment = TextTreatmentService.GetTrimMessageWithOutUserNameBot(message.Text);

        var action = messageTreatment.Split(' ')[0] switch
        {
            "/start" => Task.Run(() => new StartCommand(botClient, new(SandBoxContext.Instance))
                .Execute(message, cancellationToken), cancellationToken),
            "/ver" => Task.Run(()=> new StartCommand(botClient, new(SandBoxContext.Instance))
                .Execute(message, cancellationToken), cancellationToken),
            "/add" => Task.Run( ()=>new BlackAddCommand(botClient, new(SandBoxContext.Instance))
                .Execute(message, cancellationToken), cancellationToken),
            "/del" => Task.Run(()=>new BlackDeleteCommand(botClient, new(SandBoxContext.Instance))
                .Execute(message, cancellationToken), cancellationToken),
            "/check" => Task.Run(()=>new BlackCheckCommand(botClient, new(SandBoxContext.Instance))
                .Execute(message, cancellationToken), cancellationToken),
            "/setadmin" => Task.Run(()=>new GetAdminCommand(botClient, new(SandBoxContext.Instance))
                .Execute(message, cancellationToken), cancellationToken),
            "/setwelcome" => new SetWelcomeMessageCommand(botClient, new(SandBoxContext.Instance))
                .Execute(message, cancellationToken),
            _ => Task.Run(()=>new BlackReadAndDeleteCommand(botClient, new(SandBoxContext.Instance))
                .Execute(message, cancellationToken), cancellationToken)
        };
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    // Process Inline Keyboard callback data
    private Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
        var array = callbackQuery.Data?.Split(' ');

        if (array is null)
            return Task.CompletedTask;

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        var action = array[0] switch
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        {
            "ban" => Task.Run(()=>new BanCommand(botClient, new(SandBoxContext.Instance))
                .Execute(callbackQuery, cancellationToken), cancellationToken),
            "restore" => Task.Run(()=>new RestoreMessage(botClient, new(SandBoxContext.Instance))
                .Execute(callbackQuery, cancellationToken), cancellationToken),
        };

        return Task.CompletedTask;
    }

    #region Inline Mode

    private Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery inlineQuery,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    #endregion

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable RCS1163 // Unused parameter.
    private Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore IDE0060 // Remove unused parameter
    {
        return Task.CompletedTask;
    }
}