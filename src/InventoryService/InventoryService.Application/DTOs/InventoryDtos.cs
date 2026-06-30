namespace InventoryService.Application.DTOs;

public record AddInventoryDto(
    string ProductName,
    int Quantity,
    decimal Price
);

public record InventoryDto(
    int ProductId,
    string ProductName,
    int AvailableQuantity,
    decimal Price,
    DateTime LastUpdated
);

public record CheckInventoryDto(
    int ProductId,
    string ProductName,
    int RequestedQuantity
);

public record CheckInventoryResponseDto(
    bool IsAvailable,
    int AvailableQuantity,
    string Message
);

public record InventoryResponseDto(
    bool Success,
    string Message,
    InventoryDto? Data = null,
    string? ErrorCode = null
);