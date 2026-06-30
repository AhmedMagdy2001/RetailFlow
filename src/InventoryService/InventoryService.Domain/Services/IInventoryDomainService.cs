using Shared.Domain.Core;
using InventoryService.Domain.Entities;

namespace InventoryService.Domain.Services;

public interface IInventoryDomainService
{
    Task<Result> ValidateInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default);
    Task<Result> CheckAvailabilityAsync(int productId, int requestedQuantity, CancellationToken cancellationToken = default);
}