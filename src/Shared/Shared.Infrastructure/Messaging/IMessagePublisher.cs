namespace Shared.Infrastructure.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<T>(string queue, T message, CancellationToken cancellationToken = default);
}