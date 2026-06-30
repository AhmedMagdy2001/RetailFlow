using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.Persistence;
using Shared.Infrastructure.Messaging;

namespace OrderService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=sqlserver;Database=OrderDB;User Id=sa;Password=YourPassword123!;Encrypt=false";

        services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories
        services.AddScoped<IOrderRepository, OrderRepository>();

        // RabbitMQ
        services.AddSingleton<IConnectionFactory>(sp =>
            new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "rabbitmq",
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            });

        // Message Publisher
        services.AddScoped<IMessagePublisher, RabbitMqMessagePublisher>();

        return services;
    }
}