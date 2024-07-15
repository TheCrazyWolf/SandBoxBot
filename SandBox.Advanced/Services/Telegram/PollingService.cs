using SandBox.Advanced.Abstract;

namespace SandBox.Advanced.Services.Telegram;

// Compose Polling and ReceiverService implementations
public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
    : PollingServiceBase<ReceiverService>(serviceProvider, logger);
