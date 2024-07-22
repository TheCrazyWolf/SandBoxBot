namespace SandBox.Advanced.Configs;

public class BotConfiguration
{
    public static string UserNameBot { get; set; } = string.Empty;
    public static long IdBot { get; set; }
    public string BotToken { get; init; } = string.Empty;
    public string ManagerPasswordSecret { get; init; } = string.Empty;
    public bool IsBlockByKeywords { get; init; }
    public bool IsBlockByMachineLearn { get; init; }
    public bool IsChatInWorkTime { get; init; }
    public bool IsBlockAntiArab { get; init; }
    public bool IsBlockFastActivity { get; init; }
    public float MaxPercentageMachineLearnToBlock { get; init; }
}