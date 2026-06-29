using Shared.Domain.Core;

namespace OrderService.Domain.Entities;

/// <summary>
/// Order aggregate root - core business entity
/// </summary>
public class Order : BaseEntity
{
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public Order() { }

    public Order(string productName, int quantity, decimal price)
    {
        ProductName = productName;
        Quantity = quantity;
        Price = price;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Confirm() => Status = OrderStatus.Confirmed;
    public void Reject() => Status = OrderStatus.Rejected;
    public void Ship() => Status = OrderStatus.Shipped;

    public decimal GetTotalPrice() => Price * Quantity;
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Rejected = 2,
    Shipped = 3,
    Cancelled = 4
}