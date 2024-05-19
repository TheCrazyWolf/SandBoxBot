using SandBoxBot.Events.Base;
using Telegram.Bot;

if (!File.Exists("token.txt"))
    throw new Exception("File token.txt not found");

string token = File.ReadAllText("token.txt");

if(string.IsNullOrEmpty(token))
    throw new Exception("Token is null or empty");

var botClient = new TelegramBotClient(token);

UpdateHandler updateHandler = new UpdateHandler();

await botClient.ReceiveAsync(updateHandler);

await Task.Delay(-1);