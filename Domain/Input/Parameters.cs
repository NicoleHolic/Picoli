namespace Domain.Input;

public record struct Parameters(string[] Values)
{
    public static Parameters Empty = new([]);
    
    public bool Contains(params string[] values) => Values.Any(x => values.Contains(x));
    
    public string At(int index, string defaultValue)
    {
        return Values.ElementAtOrDefault(index) ?? defaultValue;
    }
    
    public string Rest(int from, string defaultValue)
    {
        var result = string.Join(" ", Values.Skip(from));
        return string.IsNullOrEmpty(result) ? defaultValue : result;
    }

    public T At<T>(int index, T defaultValue) where T : IParsable<T>
    {
        var val = Values.ElementAtOrDefault(index);
        return val is null ? defaultValue : T.Parse(val, null);
    }
    
    public T AtEnum<T>(int index, T defaultValue) where T : struct, Enum
    {
        var val = Values.ElementAtOrDefault(index);
        return val is null ? defaultValue : Enum.Parse<T>(val, ignoreCase: true);
    }

    public bool CountIs(int value) => Values.Length == value;
    
}