namespace Client.Data;

public record Signal
{
    public required Guid Id { get; init; }
    
    public required string Display { get; init; }
    
    public required string Name { get; init; }
    
    public required string Receiver { get; init; }
}