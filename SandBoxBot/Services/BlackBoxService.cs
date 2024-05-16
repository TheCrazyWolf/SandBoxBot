using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace SandBoxBot.Services;

public class BlackBoxService
{
    private static BlackBoxService? _instance;
    
    public static BlackBoxService Instance => _instance ??= new BlackBoxService();

    private IList<string> _blackWords;

    public BlackBoxService()
    {
        _blackWords = Read();
    }

    public void AddWord(string? value)
    {
        if(string.IsNullOrEmpty(value))
            return;
        
        if (IfExist(value))
            return;

        _blackWords.Add(value.ToLower());
        Save();
    }

    public void Remove(string value)
    {
        if (!IfExist(value))
            return;

        _blackWords.Remove(value.ToLower());
        Save();
    }

    public bool IfExist(string value)
    {
        return _blackWords.Contains(value.ToLower());
    }

    private void Save()
    {
        var json = JsonConvert.SerializeObject(_blackWords);
        File.WriteAllText("black.json", json);
    }

    private IList<string> Read()
    {
        if (!File.Exists("black.json"))
            return new Collection<string>();

        var json = File.ReadAllText("black.json");

        return JsonConvert.DeserializeObject<IList<string>>(json) ?? new Collection<string>();
    }
}