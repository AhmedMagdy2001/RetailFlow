using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Shared.Infrastructure.Messaging;

public class RabbitMqMessageConsumer : IMessageConsumer
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMqMessageConsumer> _logger;

    public RabbitMqMessageConsumer(IConnectionFactory connectionFactory, ILogger<RabbitMqMessageConsumer> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task ConsumeAsync<T>(
        string queue,
        Func<T, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
            var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                try
                {
                    var bodyBytes = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(bodyBytes);
                    var obj = JsonSerializer.Deserialize<T>(message);

                    if (obj != null)
                    {
                        await handler(obj, cancellationToken);
                    }

                    await channel.BasicAckAsync(eventArgs.DeliveryTag, false, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from queue: {Queue}", queue);
                    await channel.BasicNackAsync(eventArgs.DeliveryTag, false, true, cancellationToken);
                }
            };

            await channel.BasicConsumeAsync(queue: queue, autoAck: false, consumerTag: $"{queue}-consumer", consumer: consumer, cancellationToken: cancellationToken);

            _logger.LogInformation("Consumer started for queue: {Queue}", queue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting message consumer for queue: {Queue}", queue);
            throw;
        }
    }
}