// ReSharper disable All

using Telegram.Bot.Types;

namespace SandBox.Advanced.Configs;

public class BotConfiguration
{
    public static User BotInfo { get; set; } = new User();
    public string BotToken { get; init; } = string.Empty;
    public string ManagerPasswordSecret { get; init; } = string.Empty;
    public IList<long> AntiTelegramBotChats { get; init; } = new List<long>();
    public IList<long> AntiMediaNonTrustedUsersChats { get; init; } = new List<long>();
    public IList<long> AntiUrlsNonTrustedUsersChats { get; init; } = new List<long>();
    public IList<long> AntiSpamByBlackWordsChats { get; init; } = new List<long>();
    public IList<long> AntiSpamMachineLearnChats { get; init; } = new List<long>();
    public float MaxPercentageMachineLearnToBlock { get; init; } = 0.29f;
    public IList<long> NotifyFastActivityChats { get; init; } = new List<long>();
    public IList<long> NotifyFastJoinsChats { get; init; } = new List<long>();
    public IList<IList<long>> TrainerFaqChats { get; init; } = new List<IList<long>>();
}