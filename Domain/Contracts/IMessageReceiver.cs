using Domain.Messages;

namespace Domain.Contracts;

public interface IMessageReceiver
{
    void OnReceivingMessage(Message message);
}