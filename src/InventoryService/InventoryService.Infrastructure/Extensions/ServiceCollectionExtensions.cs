using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using InventoryService.Domain.Repositories;
using InventoryService.Infrastructure.Persistence;
using Shared.Infrastructure.Messaging;

namespace InventoryService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=sqlserver;Database=InventoryDB;User Id=sa;Password=YourPassword123!;Encrypt=false";

        services.AddDbContext<InventoryDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories
        services.AddScoped<IInventoryRepository, InventoryRepository>();

        // RabbitMQ - ADD THIS SECTION
        services.AddSingleton<IConnectionFactory>(sp =>
            new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "rabbitmq",
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            });

        // Message Consumer - ADD THIS LINE
        services.AddScoped<IMessageConsumer, RabbitMqMessageConsumer>();

        return services;
    }
}