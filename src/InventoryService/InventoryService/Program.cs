using Serilog;
using MediatR;
using Microsoft.EntityFrameworkCore;
using InventoryService.Application.Commands;
using InventoryService.Application.Extensions;
using InventoryService.Infrastructure.Extensions;
using InventoryService.Infrastructure.Persistence;
using Shared.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/inventoryservice-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Inventory Service API",
        Version = "v1",
        Description = "Inventory management microservice"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers();

var app = builder.Build();

// Migrate database with retry logic
var retryCount = 0;
const int maxRetries = 5;
while (retryCount < maxRetries)
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

            //  Migrate() automatically creates the database if missing 
            // and tracks schema changes via the history metadata table correctly.
            dbContext.Database.Migrate();

            Log.Information("InventoryDB migrations applied successfully");

            // Seed sample data
            if (!dbContext.Inventories.Any())
            {
                dbContext.Inventories.AddRange(
                    new InventoryService.Domain.Entities.Inventory { ProductId = 1, ProductName = "Laptop", AvailableQuantity = 50, Price = 999.99m },
                    new InventoryService.Domain.Entities.Inventory { ProductId = 2, ProductName = "Mouse", AvailableQuantity = 200, Price = 29.99m },
                    new InventoryService.Domain.Entities.Inventory { ProductId = 3, ProductName = "Keyboard", AvailableQuantity = 150, Price = 79.99m }
                );
                dbContext.SaveChanges();
                Log.Information("Sample inventory data seeded");
            }
        }
        break;
    }
    catch (Exception ex)
    {
        retryCount++;
        if (retryCount >= maxRetries)
        {
            Log.Fatal(ex, "Failed to migrate InventoryDB after {RetryCount} attempts", maxRetries);
            throw;
        }
        Log.Warning("Migration attempt {RetryCount} failed, retrying in 5 seconds...", retryCount);
        System.Threading.Thread.Sleep(5000);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseRouting();
app.MapControllers();

// Start message consumer before app.Run()
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStarted.Register(() =>
{
    _ = Task.Run(async () =>
    {
        try
        {
            await Task.Delay(10000); // Wait for infrastructure layout to settle
            Log.Information("Starting message consumer...");

            using (var scope = app.Services.CreateAsyncScope())
            {
                var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();

                // FIX: Pass lifetime.ApplicationStopping token so the runner shuts down gracefully
                await consumer.ConsumeAsync<OrderMessage>(
                    "order-queue",
                    async (message, ct) =>
                    {
                        try
                        {
                            using (var messageScope = app.Services.CreateAsyncScope())
                            {
                                var mediator = messageScope.ServiceProvider.GetRequiredService<IMediator>();
                                var command = new ProcessOrderCommand(message);
                                await mediator.Send(command, ct);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Error processing order message");
                        }
                    },
                    lifetime.ApplicationStopping
                );
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fatal error in message consumer");
        }
    });
});

app.Run();