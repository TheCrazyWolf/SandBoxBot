using SandBoxBot.Commands.Black;
using SandBoxBot.Commands.Keyboard.Black;
using SandBoxBot.Database;
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

        await handler;
    }

    private Task BotOnMessageReceived(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        if (message.Text is not { } messageText)
            return Task.CompletedTask;

        int atIndex = messageText.IndexOf('@');
        if (atIndex != -1)
        {
            int spaceIndex = messageText.IndexOf(' ', atIndex);
            if (spaceIndex != -1)
            {
                messageText = messageText.Substring(0, atIndex) + messageText.Substring(spaceIndex);
            }
            else
            {
                messageText = messageText.Substring(0, atIndex);
            }
        }
        
        var action = messageText.Split(' ')[0] switch
        {
            "/add" => new BlackAddCommand(botClient, new (SandBoxContext.Instance))
                .Execute(message, cancellationToken),
            "/del" => new BlackDeleteCommand(botClient, new (SandBoxContext.Instance))
                .Execute(message, cancellationToken),
            "/check" => new BlackCheckCommand(botClient, new (SandBoxContext.Instance))
                .Execute(message, cancellationToken),
            _ => new BlackReadAndDeleteCommand(botClient, new (SandBoxContext.Instance))
                .Execute(message, cancellationToken)
        };
        
        return Task.CompletedTask;
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
        
        var action = array[0] switch
        {
            "ban" => new BanCommand(botClient, new (SandBoxContext.Instance))
                .Execute(callbackQuery, cancellationToken),
            "restore" => new RestoreMessage(botClient, new (SandBoxContext.Instance))
                .Execute(callbackQuery, cancellationToken),
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