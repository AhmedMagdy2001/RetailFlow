using Serilog;
using OrderService.Application.Extensions;
using OrderService.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/orderservice-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Order Service API",
        Version = "v1",
        Description = "Order management microservice"
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
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            //  handles both physical creation 
            // and schema layout history correctly.
            dbContext.Database.Migrate();

            Log.Information("OrderDB migrations applied successfully");
        }
        break;
    }
    catch (Exception ex)
    {
        retryCount++;
        if (retryCount >= maxRetries)
        {
            Log.Fatal(ex, "Failed to migrate OrderDB after {RetryCount} attempts", maxRetries);
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

app.Run();