namespace Domain.Messages;

public record Message
{
    public required Guid Id { get; init; }
    
    public required string Topic { get; init; }
    
    public required string Sender { get; init; }
    
    public required string Receiver { get; init; }
    
    public required string Parameters { get; init; }
    
    public required DateTime PublishedAt { get; init; }
}