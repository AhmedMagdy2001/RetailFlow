using MediatR;

namespace InventoryService.Application.Commands;

public record OrderMessage(int Id, string ProductName, int Quantity, decimal Price, DateTime CreatedAt, int Status);

public record ProcessOrderCommand(OrderMessage Order) : IRequest<bool>;