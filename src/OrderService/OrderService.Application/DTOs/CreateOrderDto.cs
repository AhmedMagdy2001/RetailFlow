namespace OrderService.Application.DTOs;

public record CreateOrderDto(
    string ProductName,
    int Quantity,
    decimal Price
);

public record OrderDto(
    int Id,
    string ProductName,
    int Quantity,
    decimal Price,
    string Status,
    DateTime CreatedAt,
    decimal TotalPrice
);

public record OrderResponseDto(
    bool Success,
    string Message,
    OrderDto? Data = null,
    string? ErrorCode = null
);