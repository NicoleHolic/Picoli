namespace Domain.Input;

public record struct Parameters(string[] Values)
{
    public static Parameters Empty = new([]);
    
    public string At(int index, string defaultValue)
    {
        return Values.ElementAtOrDefault(index) ?? defaultValue;
    }

    public T At<T>(int index, T defaultValue) where T : IParsable<T>
    {
        var val = Values.ElementAtOrDefault(index);
        return val is null ? defaultValue : T.Parse(val, null);
    }

    public bool CountIs(int value) => Values.Length == value;
    
}