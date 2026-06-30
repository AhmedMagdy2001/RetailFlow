using Shared.Domain.Core;

namespace InventoryService.Domain.Entities;

public class Inventory : BaseEntity
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public decimal Price { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public Inventory() { }

    public Inventory(string productName, int availableQuantity, decimal price)
    {
        ProductName = productName;
        AvailableQuantity = availableQuantity;
        Price = price;
        LastUpdated = DateTime.UtcNow;
    }

    public bool CanFulfillOrder(int requestedQuantity)
    {
        return AvailableQuantity >= requestedQuantity;
    }

    public void DeductInventory(int quantity)
    {
        if (!CanFulfillOrder(quantity))
            throw new InvalidOperationException("Insufficient inventory");

        AvailableQuantity -= quantity;
        LastUpdated = DateTime.UtcNow;
    }

    public void AddInventory(int quantity)
    {
        AvailableQuantity += quantity;
        LastUpdated = DateTime.UtcNow;
    }
}

public class InventoryTransaction : BaseEntity
{
    public int ProductId { get; set; }
    public int QuantityChanged { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public int? OrderId { get; set; }
}