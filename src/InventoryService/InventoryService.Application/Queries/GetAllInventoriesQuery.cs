using MediatR;
using InventoryService.Application.DTOs;

namespace InventoryService.Application.Queries;

public record GetAllInventoriesQuery : IRequest<List<InventoryDto>>;