namespace SandBox.Advanced.Configs;

public class BotConfiguration
{
    public string BotToken { get; init; } = string.Empty;
    public string ManagerPasswordSecret { get; init; } = string.Empty;
    public bool IsBlockByKeywords { get; init; }
    public bool IsBlockByMachineLearn { get; init; }
    public bool IsChatInWorkTime { get; init; }
    public bool IsBlockAntiArab { get; init; }
    public bool IsBlockFastActivity { get; init; }
    public float MaxPercentageMachineLearnToBlock { get; init; }
}