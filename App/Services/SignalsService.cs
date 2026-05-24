using System.Text.Json;
using Client.Data;
using Domain.Messages;

namespace App.Services;

public sealed class SignalsService
{
    private static readonly string FilePath = FileSystem.AppDataDirectory + "/signals.json";

    private List<Signal> _signals = [];
    
    public IEnumerable<Signal> Signals => _signals;
    
    public async ValueTask LoadAsync()
    {
        if (!File.Exists(FilePath))
        {
            await SaveAsync();
            return;
        }
        
        var json = await File.ReadAllTextAsync(FilePath);
        var data = JsonSerializer.Deserialize<List<Signal>>(json);
        
        if (data is null)
            return;
        
        _signals = data;
    }
    
    public async ValueTask SaveAsync()
    {
        var json =
            JsonSerializer.Serialize(_signals);

        await File.WriteAllTextAsync(
            FilePath,
            json);
    }

    public async ValueTask InsertAsync(Signal signal)
    {
        var existing = _signals.FirstOrDefault(x => x.Id == signal.Id);
        
        if (existing is not null)
            _signals.Remove(existing);
        
        _signals.Add(signal);
        await SaveAsync();
    }
    
    public async ValueTask DeleteAsync(Signal signal)
    {
        _signals.Remove(signal);
        await SaveAsync();
    }
}