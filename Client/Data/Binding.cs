namespace Client.Data;

public record Binding
{
    public required Guid Id { get; init; }
    
    public required string Display { get; init; }
    
    public required string Name { get; init; }
    
    public required SignalType Type { get; init; }
    
    public required string Parameters { get; init; }
}