using Shared.Domain.Core;
using OrderService.Domain.Entities;

namespace OrderService.Domain.Services;

public class OrderDomainService : IOrderDomainService
{
    public Task<Result> ValidateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(order.ProductName))
            errors.Add("Product name is required");

        if (order.Quantity <= 0 || order.Quantity > 1000)
            errors.Add("Quantity must be between 1 and 1000");

        if (order.Price <= 0)
            errors.Add("Price must be greater than zero");

        if (errors.Count > 0)
            return Task.FromResult(Result.Failure("Order validation failed", "VALIDATION_ERROR", errors));

        return Task.FromResult(Result.Ok("Order is valid"));
    }

    public decimal CalculateOrderTotal(Order order)
    {
        return order.GetTotalPrice();
    }
}