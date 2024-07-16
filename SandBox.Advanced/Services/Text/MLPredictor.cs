using SandBox_Advanced;

namespace SandBox.Advanced.Services.Text;

public static class MlPredictor
{
    public static float MaxScoreToPredictIsSpam = 0.19f;
    public static (bool, float) IsSpamPredict(string? message)
    {
        // model training  lbfgsmaximumEntropyMulti
        
        var sampleData = new AntiWorkSpam.ModelInput
        {
            Value = message ?? string.Empty,
        };
        var result = AntiWorkSpam.Predict(sampleData);

        return result.Score[1] >= MaxScoreToPredictIsSpam ? (true, result.Score[1] * 100) : (false, result.Score[1] * 100);
    }
}