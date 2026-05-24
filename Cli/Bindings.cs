using System.Collections;
using System.Text.Json;
using Binding = Client.Data.Binding;

namespace Cli;

internal sealed class Bindings : IEnumerable<Binding>
{
    private const string FilePath = "bindings.json";

    private List<Binding> _values = [];
    
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
        
            _values = data;
        }
        catch (Exception e)
        {
            _values = [];
            await SaveAsync();
        }
    }
    
    public async ValueTask SaveAsync()
    {
        var json =
            JsonSerializer.Serialize(_values);

        await File.WriteAllTextAsync(
            FilePath,
            json);
    }

    public async ValueTask AddAsync(Binding binding)
    {
        var existing = _values.FirstOrDefault(x => x.Id == binding.Id);
        
        if (existing is not null)
            _values.Remove(existing);
        
        _values.Add(binding);
        await SaveAsync();
    }
    
    public async ValueTask RemoveAsync(string name)
    {
        var value = _values.FirstOrDefault(x => x.Display.Equals(name) || x.Name.Equals(name));
        
        if (value is not null)
            await RemoveAsync(value);
    }
    
    public async ValueTask RemoveAsync(Binding binding)
    {
        _values.Remove(binding);
        await SaveAsync();
    }

    public IEnumerator<Binding> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}