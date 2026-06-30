using InventoryService.Domain.Entities;

namespace InventoryService.Domain.Repositories;

public interface IInventoryRepository
{
    Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default);
    Task<Inventory?> GetByIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<List<Inventory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(Inventory inventory, CancellationToken cancellationToken = default);
    Task AddTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default);
}