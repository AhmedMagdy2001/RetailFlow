using MediatR;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Commands;

public class ProcessOrderCommandHandler : IRequestHandler<ProcessOrderCommand, bool>
{
    private readonly IInventoryRepository _repository;
    private readonly ILogger<ProcessOrderCommandHandler> _logger;

    public ProcessOrderCommandHandler(IInventoryRepository repository, ILogger<ProcessOrderCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(ProcessOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = request.Order;

            // For demo, assume productId = 1
            var inventory = await _repository.GetByIdAsync(1, cancellationToken);

            if (inventory == null)
            {
                _logger.LogWarning("Product not found: {ProductId}", 1);
                return false;
            }

            if (inventory.CanFulfillOrder(order.Quantity))
            {
                inventory.DeductInventory(order.Quantity);
                await _repository.UpdateAsync(inventory, cancellationToken);

                var transaction = new InventoryTransaction
                {
                    ProductId = inventory.ProductId,
                    QuantityChanged = -order.Quantity,
                    TransactionType = "order_fulfilled",
                    OrderId = order.Id,
                    CreatedAt = DateTime.UtcNow
                };

                await _repository.AddTransactionAsync(transaction, cancellationToken);
                _logger.LogInformation("Order {OrderId} fulfilled. Inventory deducted: {Quantity}", order.Id, order.Quantity);
                return true;
            }

            _logger.LogWarning("Insufficient inventory for order {OrderId}", order.Id);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order");
            return false;
        }
    }
}