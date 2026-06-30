using MediatR;
using InventoryService.Application.DTOs;

namespace InventoryService.Application.Queries;

public record GetInventoryQuery(int ProductId) : IRequest<InventoryResponseDto>;