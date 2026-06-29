namespace Shared.Infrastructure.Messaging;

public interface IMessageConsumer
{
    Task ConsumeAsync<T>(
        string queue,
        Func<T, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default);
}