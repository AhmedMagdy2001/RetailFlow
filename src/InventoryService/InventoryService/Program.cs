using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Connect and create channel once on startup
var factory = new ConnectionFactory
{
    HostName = "rabbitmq",
    UserName = "guest",
    Password = "guest"
};

var connection = await factory.CreateConnectionAsync();
var channel = await connection.CreateChannelAsync();

// Ensure queue exists
await channel.QueueDeclareAsync(
    queue: "order-queue",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null
);

// Event consumer
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, eventArgs) =>
{
    var bodyBytes = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(bodyBytes);
    var order = System.Text.Json.JsonSerializer.Deserialize<Order>(message);

    Console.WriteLine($"Received order -> {order?.ProductName}, Qty: {order?.Quantity}");

    // Acknowledge the message
    channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);

    await Task.CompletedTask;
};

// Start consuming
await channel.BasicConsumeAsync(
    queue: "order-queue",
    autoAck: false,
    consumer: consumer
);

app.Run();
