using Domain.Messages;

namespace Domain.Contracts;

public interface IMessagePublisher
{
    ValueTask<bool> PublishAsync(Message message);
}