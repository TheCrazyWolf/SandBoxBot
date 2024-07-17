using Microsoft.Extensions.Options;
using SandBox.Advanced;
using SandBox.Advanced.Configs;
using SandBox.Advanced.Database;
using SandBox.Advanced.Services;
using SandBox.Advanced.Services.Telegram;
using SandBox.Advanced.Services.Text;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

// Регистрируем конфигурацию из appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Регистрируем BotConfiguration как конфигурационный объект
builder.Services.Configure<BotConfiguration>(builder.Configuration.GetSection("BotConfiguration"));

// Настраиваем HttpClient для Telegram Bot
builder.Services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        var botConfiguration = sp.GetRequiredService<IOptions<BotConfiguration>>().Value;
        ArgumentNullException.ThrowIfNull(botConfiguration, nameof(botConfiguration));
        MlPredictor.MaxScoreToPredictIsSpam = botConfiguration.MaxPercentageMachineLearnToBlock;
        TelegramBotClientOptions options = new(botConfiguration.BotToken);
        return new TelegramBotClient(options, httpClient);
    });

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<ReceiverService>();
builder.Services.AddHostedService<PollingService>();
builder.Services.AddDbContext<SandBoxContext>();
builder.Services.AddScoped<SandBoxRepository>();

var host = builder.Build();
host.Run();