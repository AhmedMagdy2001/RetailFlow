using Shared.Domain.Core;
using InventoryService.Domain.Entities;

namespace InventoryService.Domain.Services;

public class InventoryDomainService : IInventoryDomainService
{
    public Task<Result> ValidateInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(inventory.ProductName))
            errors.Add("Product name is required");

        if (inventory.AvailableQuantity < 0)
            errors.Add("Quantity cannot be negative");

        if (inventory.Price <= 0)
            errors.Add("Price must be greater than zero");

        if (errors.Count > 0)
            return Task.FromResult(Result.Failure("Inventory validation failed", "VALIDATION_ERROR", errors));

        return Task.FromResult(Result.Ok("Inventory is valid"));
    }

    public Task<Result> CheckAvailabilityAsync(int productId, int requestedQuantity, CancellationToken cancellationToken = default)
    {
        // This will be implemented with repository access in service layer
        return Task.FromResult(Result.Ok("Availability check passed"));
    }
}