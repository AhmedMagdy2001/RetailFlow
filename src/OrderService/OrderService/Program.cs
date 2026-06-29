using System.Text;
using RabbitMQ.Client;
using Shared;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/orders", async (Order order) =>
{
    var factory = new ConnectionFactory
    {
        HostName = "rabbitmq",
        UserName = "guest",
        Password = "guest"
    };

    // Create connection/channel (async API)
    await using var connection = await factory.CreateConnectionAsync();
    await using var channel = await connection.CreateChannelAsync();

    // Ensure queue is declared
    await channel.QueueDeclareAsync(
        queue: "order-queue",
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null
    );

    // Serialize to bytes
    var body = Encoding.UTF8.GetBytes(
        System.Text.Json.JsonSerializer.Serialize(order)
    );

    // ? Use the simple BasicPublishAsync overload
    await channel.BasicPublishAsync(
        exchange: "",
        routingKey: "order-queue",
        body: body
    );

    return Results.Ok("Order published");
});

app.Run();
