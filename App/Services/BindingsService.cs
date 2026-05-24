using System.Text.Json;
using Binding = Client.Data.Binding;

namespace App.Services;

public sealed class BindingsService
{
    private static readonly string FilePath = FileSystem.AppDataDirectory + "/bindings.json";

    private List<Binding> _bindings = [];
    
    public IEnumerable<Binding> Bindings => _bindings;
    
    public async ValueTask LoadAsync()
    {
        if (!File.Exists(FilePath))
        {
            await SaveAsync();
            return;
        }
        
        try
        {
            var json = await File.ReadAllTextAsync(FilePath);
            var data = JsonSerializer.Deserialize<List<Binding>>(json);
            
            if (data is null)
                return;
        
            _bindings = data;
        }
        catch (Exception e)
        {
            _bindings = [];
            await SaveAsync();
        }
    }
    
    public async ValueTask SaveAsync()
    {
        var json =
            JsonSerializer.Serialize(_bindings);

        await File.WriteAllTextAsync(
            FilePath,
            json);
    }

    public async ValueTask InsertAsync(Binding binding)
    {
        var existing = _bindings.FirstOrDefault(x => x.Id == binding.Id);
        
        if (existing is not null)
            _bindings.Remove(existing);
        
        _bindings.Add(binding);
        await SaveAsync();
    }
    
    public async ValueTask DeleteAsync(Binding binding)
    {
        _bindings.Remove(binding);
        await SaveAsync();
    }
}