using SandBoxBot.Events.Base;
using Telegram.Bot;

string token = "";

var botClient = new TelegramBotClient(token);

UpdateHandler updateHandler = new UpdateHandler();

await botClient.ReceiveAsync(updateHandler);

await Task.Delay(-1);