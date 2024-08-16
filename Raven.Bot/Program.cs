using Raven.Bot;
using Raven.Bot.Implementation;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();


builder.Services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        var botToken = sp.GetRequiredService<IConfiguration>().GetValue<string>("BotToken");
        ArgumentNullException.ThrowIfNull(botToken, nameof(botToken));
        TelegramBotClientOptions options = new(botToken);
        return new TelegramBotClient(options, httpClient);
    });

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<ReceiverService>();
builder.Services.AddHostedService<PollingService>();

var host = builder.Build();
host.Run();