using Microsoft.EntityFrameworkCore;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Repositories;

namespace InventoryService.Infrastructure.Persistence;

public class InventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _dbContext;

    public InventoryRepository(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        _dbContext.Inventories.Add(inventory);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Inventory?> GetByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);
    }

    public async Task<List<Inventory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Inventories.ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        _dbContext.Inventories.Update(inventory);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default)
    {
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}