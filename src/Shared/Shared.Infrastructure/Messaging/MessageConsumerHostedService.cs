using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.Infrastructure.Messaging;

public class MessageConsumerHostedService : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly ILogger<MessageConsumerHostedService> _logger;

    public MessageConsumerHostedService(IMessageConsumer consumer, ILogger<MessageConsumerHostedService> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Optional: short initial delay to let infrastructure spin up
            await Task.Delay(2000, stoppingToken);
            _logger.LogInformation("Starting message consumer background service...");

            // Replace 'OrderMessage' with your actual concrete C# class/DTO structure instead of dynamic!
            await _consumer.ConsumeAsync<dynamic>(
                "order-queue",
                async (message, ct) =>
                {
                    _logger.LogInformation("Processing order ID: {OrderId}", (object)message.Id);
                    await Task.CompletedTask;
                },
                stoppingToken
            );
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Message consumer background service stopped cleanly via token.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in message consumer hosted service execution loop.");
        }
    }
}

