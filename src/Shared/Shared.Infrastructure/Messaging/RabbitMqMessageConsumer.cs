using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization; // Added for JsonNumberHandling
using Microsoft.Extensions.Logging;

namespace Shared.Infrastructure.Messaging;

public class RabbitMqMessageConsumer : IMessageConsumer
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMqMessageConsumer> _logger;

    // Define resilient JSON options globally for the consumer
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
    };

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
        IConnection connection = null;
        IChannel channel = null;

        try
        {
            int retryCount = 0;
            const int maxRetries = 10;
            const int retryDelayMs = 2000;

            while (connection == null)
            {
                try
                {
                    connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
                }
                catch (Exception ex) when (retryCount < maxRetries && !cancellationToken.IsCancellationRequested)
                {
                    retryCount++;
                    _logger.LogWarning(ex, "RabbitMQ connection attempt {RetryCount} failed. Retrying in {Delay}ms...", retryCount, retryDelayMs);
                    await Task.Delay(retryDelayMs, cancellationToken);
                }
            }

            channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

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
                    _logger.LogInformation("Message received from queue: {Queue}", queue);

                    var bodyBytes = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(bodyBytes);

                    //  Pass resilient serializer options here
                    var obj = JsonSerializer.Deserialize<T>(message, SerializerOptions);

                    if (obj != null)
                    {
                        await handler(obj, cancellationToken);
                        _logger.LogInformation("Message processed successfully");
                    }

                    await channel.BasicAckAsync(eventArgs.DeliveryTag, false, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from queue: {Queue}", queue);
                    try
                    {
                        // AMEND: Change 'requeue' parameter from true to false for JSON format exceptions
                        // This prevents poison messages from causing an infinite loop.
                        bool shouldRequeue = ex is not JsonException;

                        await channel.BasicNackAsync(eventArgs.DeliveryTag, false, shouldRequeue, cancellationToken);
                    }
                    catch (Exception nackEx)
                    {
                        _logger.LogError(nackEx, "Failed to send NACK back to broker.");
                    }
                }
            };

            await channel.BasicConsumeAsync(
                queue: queue,
                autoAck: false,
                consumerTag: $"{queue}-consumer",
                consumer: consumer,
                cancellationToken: cancellationToken
            );

            _logger.LogInformation("Consumer successfully attached to queue: {Queue}", queue);

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer cancellation requested for queue: {Queue}.", queue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error running message consumer for queue: {Queue}", queue);
            throw;
        }
        finally
        {
            if (channel != null)
            {
                await channel.CloseAsync();
                await channel.DisposeAsync();
            }
            if (connection != null)
            {
                await connection.CloseAsync();
                await connection.DisposeAsync();
            }
        }
    }
}